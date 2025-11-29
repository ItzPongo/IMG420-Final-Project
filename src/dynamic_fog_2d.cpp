#include "dynamic_fog_2d.h"

void DynamicFog2D::_bind_methods() {
    // Core fog update methods
    ClassDB::bind_method(D_METHOD("UpdateFog"), &DynamicFog2D::UpdateFog);
    ClassDB::bind_method(D_METHOD("GetRotatedLightImage", "light_sprite"), &DynamicFog2D::GetRotatedLightImage);
    ClassDB::bind_method(D_METHOD("RegenerateFog", "delta_time"), &DynamicFog2D::RegenerateFog);
    ClassDB::bind_method(D_METHOD("UpdateFogTexture"), &DynamicFog2D::UpdateFogTexture);

    // Property getters/setters
    ClassDB::bind_method(D_METHOD("set_display_width", "width"), &DynamicFog2D::set_display_width);
    ClassDB::bind_method(D_METHOD("get_display_width"), &DynamicFog2D::get_display_width);
    ClassDB::bind_method(D_METHOD("set_display_height", "height"), &DynamicFog2D::set_display_height);
    ClassDB::bind_method(D_METHOD("get_display_height"), &DynamicFog2D::get_display_height);
    ClassDB::bind_method(D_METHOD("set_fog_regen_rate", "rate"), &DynamicFog2D::set_fog_regen_rate);
    ClassDB::bind_method(D_METHOD("get_fog_regen_rate"), &DynamicFog2D::get_fog_regen_rate);
    ClassDB::bind_method(D_METHOD("set_light_group", "group_name"), &DynamicFog2D::set_light_group);
    ClassDB::bind_method(D_METHOD("get_light_group"), &DynamicFog2D::get_light_group);

    // Register properties in the editor
    ADD_PROPERTY(PropertyInfo(Variant::INT, "display_width"), "set_display_width", "get_display_width");
    ADD_PROPERTY(PropertyInfo(Variant::INT, "display_height"), "set_display_height", "get_display_height");
    ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "FogRegenRate"), "set_fog_regen_rate", "get_fog_regen_rate");
    ADD_PROPERTY(PropertyInfo(Variant::STRING, "LightGroup"), "set_light_group", "get_light_group");
}

// Constructor
DynamicFog2D::DynamicFog2D() {}

// Called when the node enters the scene tree
void DynamicFog2D::_ready() {
    bool is_editor_mode = Engine::get_singleton()->is_editor_hint();

    // Attempt to get the child Sprite2D node named "Fog"
    fog_sprite = Object::cast_to<Sprite2D>(get_node_or_null("Fog"));
    if (!fog_sprite) {
        print_line("[DynamicFog2D] Error: No child Sprite2D named 'Fog' found.");
        print_line("Please add a Sprite2D node named 'Fog' as a child.");
        set_process(false);
        return;
    }

    // Disable processing while in editor
    if (is_editor_mode) {
        set_process(false);
        return;
    }

    // Calculate fog image size based on display dimensions and grid size
    int fog_image_width = display_width / GRID_SIZE;
    int fog_image_height = display_height / GRID_SIZE;
    fog_image_width = fog_image_width > 0 ? fog_image_width : 1;
    fog_image_height = fog_image_height > 0 ? fog_image_height : 1;

    // Create the fog image and fill with full black (opaque)
    fog_image.instantiate();
    fog_image = fog_image->create_empty(fog_image_width, fog_image_height, false, Image::FORMAT_RGBA8);
    fog_image->fill(Color(0, 0, 0, 1));

    // Adjust the fog sprite scale and position to match display
    Vector2 original_scale = fog_sprite->get_scale();
    fog_sprite->set_scale(Vector2(original_scale.x * GRID_SIZE, original_scale.y * GRID_SIZE));
    fog_sprite->set_position(Vector2(display_width / 2.0f, display_height / 2.0f));

    // Push initial texture to the sprite
    UpdateFogTexture();

    set_process(true);
}

// Main per-frame update
void DynamicFog2D::_process(double delta_time) {
    RegenerateFog((float)delta_time);
    UpdateFog();
}

// Gradually restore fog opacity over time
void DynamicFog2D::RegenerateFog(float delta_time) {
    if (fog_image.is_null()) return;

    int width = fog_image->get_width();
    int height = fog_image->get_height();
    if (width <= 0 || height <= 0) return;

    float regen_amount = fog_regen_rate * delta_time;

    for (int y = 0; y < height; y++) {
        for (int x = 0; x < width; x++) {
            Color pixel_color = fog_image->get_pixel(x, y);
            pixel_color.r = UtilityFunctions::move_toward(pixel_color.r, 0.0f, regen_amount);
            pixel_color.g = UtilityFunctions::move_toward(pixel_color.g, 0.0f, regen_amount);
            pixel_color.b = UtilityFunctions::move_toward(pixel_color.b, 0.0f, regen_amount);
            pixel_color.a = UtilityFunctions::move_toward(pixel_color.a, 1.0f, regen_amount);
            fog_image->set_pixel(x, y, pixel_color);
        }
    }
}

// Apply lighting from all nodes in the assigned light group
void DynamicFog2D::UpdateFog() {
    if (light_group.is_empty() || fog_image.is_null() || !fog_sprite) return;

    SceneTree *scene_tree = get_tree();
    if (!scene_tree) return;

    int fog_width = fog_image->get_width();
    int fog_height = fog_image->get_height();
    if (fog_width <= 0 || fog_height <= 0) return;

    Array lights = scene_tree->get_nodes_in_group(light_group);
    if (lights.is_empty()) return;

    // Iterate over all lights and blend their textures into the fog
    for (int i = 0; i < lights.size(); i++) {
        Object *obj = Object::cast_to<Object>(lights[i]);
        if (!obj) continue;

        Sprite2D *light_sprite = Object::cast_to<Sprite2D>(obj);
        if (!light_sprite) continue;

        Ref<Texture2D> light_texture = light_sprite->get_texture();
        if (light_texture.is_null()) continue;

        Ref<Image> light_image;
        bool has_valid_image = false;

        if (light_texture->has_method("get_image")) {
            light_image = light_texture->get_image();
            if (!light_image.is_null() && light_image->get_width() > 0 && light_image->get_height() > 0) {
                has_valid_image = true;
            } else {
                light_image.unref();
            }
        }
        if (!has_valid_image) continue;

        // Rotate and scale the light texture as needed
        Ref<Image> rotated_light_image = GetRotatedLightImage(light_sprite);
        if (rotated_light_image.is_null()) continue;

        int light_width = rotated_light_image->get_width();
        int light_height = rotated_light_image->get_height();
        if (light_width <= 0 || light_height <= 0) continue;

        Vector2 half_size((float)light_width * 0.5f, (float)light_height * 0.5f);

        Vector2 local_light_pos = light_sprite->get_global_position() / GRID_SIZE;
        Rect2i blend_rect(0, 0, light_width, light_height);
        Vector2i blend_destination(
            Math::round(local_light_pos.x - half_size.x + light_sprite->get_offset().x * light_sprite->get_scale().x),
            Math::round(local_light_pos.y - half_size.y + light_sprite->get_offset().y * light_sprite->get_scale().y)
        );

        // Blend the rotated light into the fog image
        fog_image->blend_rect(rotated_light_image, blend_rect, blend_destination);
    }

    // Update the fog sprite with the new texture
    UpdateFogTexture();
}

// Rotate and scale a light's image to its Sprite2D properties for blending
Ref<Image> DynamicFog2D::GetRotatedLightImage(Sprite2D *light_sprite) {
    if (!light_sprite) return Ref<Image>();

    Ref<Texture2D> texture = light_sprite->get_texture();
    if (texture.is_null()) return Ref<Image>();

    Ref<Image> source_image = texture->get_image();
    if (source_image.is_null()) return Ref<Image>();

    source_image->convert(Image::FORMAT_RGBA8);

    int src_width = source_image->get_width();
    int src_height = source_image->get_height();
    if (src_width <= 0 || src_height <= 0) return Ref<Image>();

    float rotation_radians = Math::deg_to_rad(light_sprite->get_global_rotation_degrees());
    Vector2 scale = light_sprite->get_scale();
    scale.x = (scale.x == 0.0f) ? 1.0f : scale.x;
    scale.y = (scale.y == 0.0f) ? 1.0f : scale.y;

    // Calculate dimensions of the rotated image
    float cos_angle = Math::abs(Math::cos(rotation_radians));
    float sin_angle = Math::abs(Math::sin(rotation_radians));
    int rotated_width = (int)Math::ceil(src_width * scale.x * cos_angle + src_height * scale.y * sin_angle);
    int rotated_height = (int)Math::ceil(src_width * scale.x * sin_angle + src_height * scale.y * cos_angle);

    // Create and fill the rotated image with transparent black
    Ref<Image> rotated_image;
    rotated_image.instantiate();
    rotated_image = rotated_image->create_empty(rotated_width, rotated_height, false, Image::FORMAT_RGBA8);
    rotated_image->fill(Color(0, 0, 0, 0));

    Vector2 src_center(src_width / 2.0f, src_height / 2.0f);
    Vector2 dst_center(rotated_width / 2.0f, rotated_height / 2.0f);

    // Map pixels from source to rotated image
    for (int y = 0; y < rotated_height; y++) {
        for (int x = 0; x < rotated_width; x++) {
            Vector2 dst_pos((float)x, (float)y);
            Vector2 rel_pos = dst_pos - dst_center;
            Vector2 src_pos = rel_pos.rotated(-rotation_radians);
            src_pos.x /= scale.x;
            src_pos.y /= scale.y;
            src_pos += src_center;

            int sx = (int)Math::floor(src_pos.x);
            int sy = (int)Math::floor(src_pos.y);

            if (sx >= 0 && sx < src_width && sy >= 0 && sy < src_height) {
                rotated_image->set_pixel(x, y, source_image->get_pixel(sx, sy));
            }
        }
    }

    return rotated_image;
}

// Push the current fog image to the fog sprite
void DynamicFog2D::UpdateFogTexture() {
    if (!fog_sprite || fog_image.is_null()) return;

    int width = fog_image->get_width();
    int height = fog_image->get_height();
    if (width <= 0 || height <= 0) return;

    Ref<ImageTexture> texture = ImageTexture::create_from_image(fog_image);
    if (texture.is_null()) return;

    fog_texture = texture;
    fog_sprite->set_texture(fog_texture);
    fog_sprite->set_visible(true);
}

// Property getters/setters
void DynamicFog2D::set_display_width(int width) { display_width = width; }
int DynamicFog2D::get_display_width() const { return display_width; }

void DynamicFog2D::set_display_height(int height) { display_height = height; }
int DynamicFog2D::get_display_height() const { return display_height; }

void DynamicFog2D::set_fog_regen_rate(float rate) { fog_regen_rate = rate; }
float DynamicFog2D::get_fog_regen_rate() const { return fog_regen_rate; }

void DynamicFog2D::set_light_group(const String &group_name) { light_group = group_name; }
String DynamicFog2D::get_light_group() const { return light_group; }

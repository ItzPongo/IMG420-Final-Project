#ifndef DYNAMIC_FOG_2D_H
#define DYNAMIC_FOG_2D_H

#include <godot_cpp/classes/node2d.hpp>
#include <godot_cpp/classes/sprite2d.hpp>
#include <godot_cpp/classes/image.hpp>
#include <godot_cpp/classes/image_texture.hpp>
#include <godot_cpp/classes/scene_tree.hpp>
#include <godot_cpp/classes/engine.hpp>
#include <godot_cpp/classes/global_constants.hpp>
#include <godot_cpp/variant/utility_functions.hpp>

using namespace godot;

class DynamicFog2D : public Node2D {
    GDCLASS(DynamicFog2D, Node2D)

private:
    int display_width = 1152;
    int display_height = 648;
    float fog_regen_rate = 3.0f;
    String light_group = "";
    static const int GRID_SIZE = 4;

    Sprite2D *fog_sprite = nullptr;
    Ref<Image> fog_image;
    Ref<ImageTexture> fog_texture;

protected:
    static void _bind_methods();

public:
    DynamicFog2D();
    void _ready() override;
    void _process(double delta) override;

    void UpdateFog();
    Ref<Image> GetRotatedLightImage(Sprite2D *light);
    void RegenerateFog(float delta);
    void UpdateFogTexture();

    void set_display_width(int v);
    int get_display_width() const;

    void set_display_height(int v);
    int get_display_height() const;

    void set_fog_regen_rate(float v);
    float get_fog_regen_rate() const;

    void set_light_group(const String &v);
    String get_light_group() const;
};

#endif

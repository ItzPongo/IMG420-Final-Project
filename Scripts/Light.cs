using Godot;

public partial class Light : Sprite2D
{
	[Export] public Node2D Pivot;
	[Export] public CollisionPolygon2D LightHitbox;
	[Export] public float Radius = 150f;
	
	private float batteryDecimal = 0f;
	
	private float baseRadius;
	private Vector2 baseScale;
	
	public override void _Ready()
	{
		baseRadius = Radius;
		baseScale = Scale;
		Position = new Vector2(576, 324);
		Pivot.Position = new Vector2(576, 324);
	}

	public override void _Process(double delta)
	{
		if (Pivot == null)
			return;

		Vector2 pivotPos = Pivot.GlobalPosition;
		Vector2 mousePos = GetGlobalMousePosition();

		Vector2 dir = mousePos - pivotPos;

		if (dir.LengthSquared() < 0.00001f)
			return;

		float angle = dir.Angle();

		GlobalPosition = pivotPos + new Vector2(Radius, 0).Rotated(angle);

		Rotation = angle;
		
		LightHitbox.Position = Position;
		LightHitbox.Rotation = Rotation;
		LightHitbox.Scale = Scale * 4.0f;
	}
	
	private void OnHudBatteryUpdate(float batteryPercent)
	{
		batteryDecimal = batteryPercent / 10000f;
		Radius = baseRadius * batteryDecimal;
		Scale = baseScale * batteryDecimal;
	}
}

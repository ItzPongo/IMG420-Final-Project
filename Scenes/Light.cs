using Godot;

public partial class Light : Sprite2D
{
	[Export] public Node2D Pivot;
	[Export] public float Radius = 150f;

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
	}
}

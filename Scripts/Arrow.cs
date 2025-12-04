using Godot;

public partial class Arrow : Sprite2D
{
	[Export] public Node2D Pivot;
	[Export] public NodePath SpawnerPath;

	[Export] public float OrbitRadius = 80f;
	[Export] public float HideDistance = 50f;
	[Export] public float SpriteAngleOffsetDegrees = 45f;

	private ItemSpawnArea _spawner;

	public override void _Ready()
	{
		if (SpawnerPath != null && !SpawnerPath.IsEmpty)
		{
			_spawner = GetNode<ItemSpawnArea>(SpawnerPath);
		}
	}

	public override void _Process(double delta)
	{
		if (_spawner == null || Pivot == null)
		{
			Visible = false;
			return;
		}

		var target = _spawner.CurrentItem;

		if (target == null || !GodotObject.IsInstanceValid(target))
		{
			Visible = false;
			return;
		}

		Vector2 pivotPos = Pivot.GlobalPosition;
		Vector2 targetPos = target.GlobalPosition;

		float dist = pivotPos.DistanceTo(targetPos);

		if (dist <= HideDistance)
		{
			Visible = false;
			return;
		}

		Visible = true;

		Vector2 dir = (targetPos - pivotPos).Normalized();

		GlobalPosition = pivotPos + dir * OrbitRadius;

		float angle = dir.Angle();
		float offset = Mathf.DegToRad(SpriteAngleOffsetDegrees);
		Rotation = angle - offset;
	}
}

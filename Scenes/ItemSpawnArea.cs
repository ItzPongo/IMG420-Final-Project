using Godot;

public partial class ItemSpawnArea : Area2D
{
	[Export] public PackedScene ItemScene;        // The item scene to spawn
	[Export] public float SpawnInterval = 1.5f;   // Seconds between spawns
	[Export] public int MaxItems = 30;            // Optional: cap total items

	private CircleShape2D _circleShape;
	private RandomNumberGenerator _rng = new();
	private float _timer = 0f;
	private int _spawnedCount = 0;

	public override void _Ready()
	{
		_rng.Randomize();

		var collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		_circleShape = collisionShape.Shape as CircleShape2D;

		if (_circleShape == null)
		{
			GD.PushError("ItemSpawnArea: CollisionShape2D must use CircleShape2D.");
		}
	}

	public override void _Process(double delta)
	{
		if (ItemScene == null || _circleShape == null)
			return;

		if (_spawnedCount >= MaxItems)
			return;

		_timer += (float)delta;

		if (_timer >= SpawnInterval)
		{
			_timer = 0f;
			SpawnItemInRadius();
		}
	}

	private void SpawnItemInRadius()
	{
		// Random point inside the circle defined by the Area2D's shape radius
		float radius = _circleShape.Radius;
		float angle = _rng.RandfRange(0, Mathf.Tau);
		float distance = _rng.RandfRange(0, radius);

		Vector2 offset = new Vector2(
			Mathf.Cos(angle) * distance,
			Mathf.Sin(angle) * distance
		);

		Node2D item = ItemScene.Instantiate<Node2D>();
		item.GlobalPosition = GlobalPosition + offset;

		GetTree().CurrentScene.AddChild(item);
		_spawnedCount++;
	}
}

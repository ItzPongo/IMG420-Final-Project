using Godot;

public partial class ItemSpawnArea : Area2D
{
	[Export] public PackedScene ItemScene;        // The item scene to spawn
	[Export] public NodePath PlayerPath;
	
	[Export] public float SpawnInterval = 20f;
	[Export] public float SpawnDistance = 300f;
	[Export] public int MaxItemsSpawned = 30;
	
	private Node2D _player;
	private Node2D _currentItem;
	private RandomNumberGenerator _rng = new();
	private float _timer = 0f;
	
	public Node2D CurrentItem => _currentItem;
	
	public override void _Ready()
	{
		_rng.Randomize();
		_player = GetNode<CharacterBody2D>(PlayerPath);
	}
	
	public override void _Process(double delta)
	{
		if (_player == null || ItemScene == null)
		{
			return;
		}
		if (_currentItem != null && GodotObject.IsInstanceValid(_currentItem))
		{
			return;
		}
		
		_timer += (float)delta;
		
		if (_timer >= SpawnInterval)
		{
			_timer = 0f;
			SpawnItemAtDistance();
		}
	}
	
	private void SpawnItemAtDistance()
	{
		float angle = _rng.RandfRange(0, Mathf.Tau);
		
		Vector2 offset = new Vector2(
			Mathf.Cos(angle) * SpawnDistance,
			Mathf.Sin(angle) * SpawnDistance
		);
		
		Node2D item = ItemScene.Instantiate<Node2D>();
		item.GlobalPosition = _player.GlobalPosition + offset;
		
		GetTree().CurrentScene.AddChild	(item);
		
		_currentItem = item;
	}
}

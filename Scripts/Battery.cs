// res://Scripts/ItemPickup.cs
using Godot;

public partial class Battery : Area2D
{
	[Export] public float ChargeAmount = 25f; // how much this item gives

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is Player player)
		{
			player.AddBattery(ChargeAmount);  // tell the player to increment their counter
			QueueFree();             // remove the item from the scene
		}
	}
}

using Godot;

public partial class BTSneak : BTNode
{
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		if (agent is not CharacterBody2D character)
		{
			GD.Print("BTSneak: Agent is not CharacterBody2D");
			return BTState.Failure;
		}
			
		CharacterBody2D player = blackboard.GetValue<CharacterBody2D>("Player");
		if (player == null || !IsInstanceValid(player))
		{
			GD.Print("BTSneak: Player is null or invalid");
			return BTState.Failure;
		}
		
		float sneakSpeed = blackboard.GetValue<float>("SneakSpeed", 30f);
		
		// Move silently toward player
		Vector2 direction = (player.GlobalPosition - character.GlobalPosition).Normalized();
		character.Velocity = direction * sneakSpeed;
		character.MoveAndSlide();
		
		GD.Print($"BTSneak: Moving towards player at speed {sneakSpeed}, Velocity: {character.Velocity}");
		
		blackboard.SetValue("CurrentState", "Sneaking");
		
		return BTState.Running;
	}
}

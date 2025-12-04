using Godot;

public partial class BTSneak : BTNode
{
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		if (agent is not CharacterBody2D character)
		{
			return BTState.Failure;
		}
			
		CharacterBody2D player = blackboard.GetValue<CharacterBody2D>("Player");
		if (player == null || !IsInstanceValid(player))
		{
			return BTState.Failure;
		}
		
		float sneakSpeed = blackboard.GetValue<float>("SneakSpeed", 30f);
		
		// Move silently toward player
		Vector2 direction = (player.GlobalPosition - character.GlobalPosition).Normalized();
		character.Velocity = direction * sneakSpeed;
		character.MoveAndSlide();
		
		blackboard.SetValue("CurrentState", "Sneaking");
		
		return BTState.Running;
	}
}

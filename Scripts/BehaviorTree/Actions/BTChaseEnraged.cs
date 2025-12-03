using Godot;

public partial class BTChaseEnraged : BTNode
{
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		if (agent is not CharacterBody2D character)
			return BTState.Failure;
			
		CharacterBody2D player = blackboard.GetValue<CharacterBody2D>("Player");
		if (player == null || !IsInstanceValid(player))
			return BTState.Failure;
		
		float enragedSpeed = blackboard.GetValue<float>("EnragedSpeed", 150f);
		
		// Chase player aggressively
		Vector2 direction = (player.GlobalPosition - character.GlobalPosition).Normalized();
		character.Velocity = direction * enragedSpeed;
		character.MoveAndSlide();
		
		blackboard.SetValue("CurrentState", "Enraged");
		
		return BTState.Running;
	}
}

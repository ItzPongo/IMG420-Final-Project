using Godot;

public partial class BTEvade : BTNode
{
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		if (agent is not CharacterBody2D character)
			return BTState.Failure;
			
		CharacterBody2D player = blackboard.GetValue<CharacterBody2D>("Player");
		if (player == null || !IsInstanceValid(player))
			return BTState.Failure;
		
		float evadeSpeed = blackboard.GetValue<float>("EvadeSpeed", 100f);
		float delta = blackboard.GetValue<float>("Delta", 0.0f);
		float evadeTimer = blackboard.GetValue<float>("EvadeTimer", 0.0f);
		
		// Countdown evade timer
		evadeTimer -= delta;
		
		// If timer hits 0, stop evading
		if (evadeTimer <= 0.0f)
		{
			evadeTimer = 0.0f;
			blackboard.SetValue("EvadeTimer", evadeTimer);
			return BTState.Success;
		}
		
		blackboard.SetValue("EvadeTimer", evadeTimer);
		
		// Move away from player
		Vector2 fleeDirection = (character.GlobalPosition - player.GlobalPosition).Normalized();
		character.Velocity = fleeDirection * evadeSpeed;
		character.MoveAndSlide();
		
		blackboard.SetValue("CurrentState", "Evading");
		
		return BTState.Running;
	}
}

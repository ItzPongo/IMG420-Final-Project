using Godot;

public partial class BTKillPlayer : BTNode
{
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		// Check if player is already dead
		bool playerIsDead = blackboard.GetValue<bool>("PlayerIsDead", false);
		if (playerIsDead)
		{
			return BTState.Success;
		}
		
		if (agent is not CharacterBody2D character)
		{
			return BTState.Failure;
		}
			
		CharacterBody2D player = blackboard.GetValue<CharacterBody2D>("Player");
		if (player == null || !IsInstanceValid(player))
		{
			return BTState.Failure;
		}
		
		// Stop movement
		character.Velocity = Vector2.Zero;
		
		// Mark player as dead in blackboard to prevent multiple calls
		blackboard.SetValue("PlayerIsDead", true);
		
		// Kill player
		if (player is Player playerScript)
		{
			playerScript.Die();
		}
		else if (player.HasMethod("Die"))
		{
			player.Call("Die");
		}
		else
		{
			blackboard.SetValue("PlayerIsDead", false);
			return BTState.Failure;
		}
		
		blackboard.SetValue("CurrentState", "Attacking");
		
		return BTState.Success;
	}
}

using Godot;

public partial class BTKillPlayer : BTNode
{
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		if (agent is not CharacterBody2D character)
			return BTState.Failure;
			
		CharacterBody2D player = blackboard.GetValue<CharacterBody2D>("Player");
		if (player == null || !IsInstanceValid(player))
			return BTState.Failure;
		
		// Stop movement
		character.Velocity = Vector2.Zero;
		
		// Kill player
		if (player.HasMethod("Die"))
		{
			player.Call("Die");
		}
		
		blackboard.SetValue("CurrentState", "Attacking");
		
		GD.Print("Player killed by Enemy!");
		
		return BTState.Success;
	}
}

using Godot;

public partial class BTIsBehindPlayer : BTNode
{
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		if (agent is not Node2D agentNode)
			return BTState.Failure;
			
		CharacterBody2D player = blackboard.GetValue<CharacterBody2D>("Player");
		if (player == null || !IsInstanceValid(player))
			return BTState.Failure;
		
		// Calculate if enemy is behind player
		Vector2 toEnemy = (agentNode.GlobalPosition - player.GlobalPosition).Normalized();
		Vector2 playerFacing = Vector2.Right;
		
		// Dot product < 0 means behind
		bool isBehind = toEnemy.Dot(playerFacing) < 0;
		
		return isBehind ? BTState.Success : BTState.Failure;
	}
}

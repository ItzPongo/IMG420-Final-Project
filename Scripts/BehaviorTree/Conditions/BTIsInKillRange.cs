using Godot;

public partial class BTIsInKillRange : BTNode
{
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		if (agent is not Node2D agentNode)
			return BTState.Failure;
			
		CharacterBody2D player = blackboard.GetValue<CharacterBody2D>("Player");
		if (player == null || !IsInstanceValid(player))
			return BTState.Failure;
		
		float killRange = blackboard.GetValue<float>("KillRange", 30f);
		float distance = agentNode.GlobalPosition.DistanceTo(player.GlobalPosition);
		
		return distance <= killRange ? BTState.Success : BTState.Failure;
	}
}

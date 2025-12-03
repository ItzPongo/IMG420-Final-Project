using Godot;

public partial class BTIsEvadeTimerActive : BTNode
{
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		float evadeTimer = blackboard.GetValue<float>("EvadeTimer", 0.0f);
		return evadeTimer > 0.0f ? BTState.Success : BTState.Failure;
	}
}

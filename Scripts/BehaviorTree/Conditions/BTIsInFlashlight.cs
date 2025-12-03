using Godot;

public partial class BTIsInFlashlight : BTNode
{
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		bool isInFlashlight = blackboard.GetValue<bool>("IsInFlashlight", false);
		return isInFlashlight ? BTState.Success : BTState.Failure;
	}
}

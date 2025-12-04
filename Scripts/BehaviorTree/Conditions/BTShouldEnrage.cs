using Godot;

public partial class BTShouldEnrage : BTNode
{
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		bool isEnraged = blackboard.GetValue<bool>("IsEnraged", false);
		
		// If enraged, always succeed to continue the enraged chase
		return isEnraged ? BTState.Success : BTState.Failure;
	}
}

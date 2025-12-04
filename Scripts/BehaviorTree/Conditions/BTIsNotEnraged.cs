using Godot;

public partial class BTIsNotEnraged : BTNode
{
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		bool isEnraged = blackboard.GetValue<bool>("IsEnraged", false);
		
		// Return Success if NOT enraged (allows the sequence to continue)
		// Return Failure if enraged (blocks the sequence)
		return isEnraged ? BTState.Failure : BTState.Success;
	}
}

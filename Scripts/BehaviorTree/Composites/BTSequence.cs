using Godot;

public partial class BTSequence : BTNode
{
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		foreach (Node child in GetChildren())
		{
			if (child is BTNode btNode)
			{
				BTState state = btNode.Tick(agent, blackboard);
				
				if (state == BTState.Failure)
					return BTState.Failure;
					
				if (state == BTState.Running)
					return BTState.Running;
			}
		}
		
		return BTState.Success;
	}
}

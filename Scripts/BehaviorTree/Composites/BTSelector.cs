using Godot;

public partial class BTSelector : BTNode
{
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		foreach (Node child in GetChildren())
		{
			if (child is BTNode btNode)
			{
				BTState state = btNode.Tick(agent, blackboard);
				
				if (state == BTState.Success)
					return BTState.Success;
					
				if (state == BTState.Running)
					return BTState.Running;
			}
		}
		
		return BTState.Failure;
	}
}

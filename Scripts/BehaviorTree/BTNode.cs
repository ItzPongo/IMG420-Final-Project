using Godot;

public enum BTState
{
	Running,
	Success,
	Failure
}

public partial class BTNode : Node
{
	public virtual BTState Tick(Node agent, Blackboard blackboard)
	{
		return BTState.Failure;
	}

	public virtual void Initialize(Node agent, Blackboard blackboard)
	{
		foreach (Node child in GetChildren())
		{
			if (child is BTNode btChild)
			{
				btChild.Initialize(agent, blackboard);
			}
		}
	}
}

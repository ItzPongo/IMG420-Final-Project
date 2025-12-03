using Godot;

public partial class BTSelector : BTNode
{
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		GD.Print("BTSelector: Ticking...");
		foreach (Node child in GetChildren())
		{
			if (child is BTNode btNode)
			{
				GD.Print($"BTSelector: Checking child {child.Name}");
				BTState state = btNode.Tick(agent, blackboard);
				
				GD.Print($"BTSelector: Child {child.Name} returned {state}");
				
				if (state == BTState.Success)
					return BTState.Success;
					
				if (state == BTState.Running)
					return BTState.Running;
			}
		}
		
		GD.Print("BTSelector: All children failed");
		return BTState.Failure;
	}
}

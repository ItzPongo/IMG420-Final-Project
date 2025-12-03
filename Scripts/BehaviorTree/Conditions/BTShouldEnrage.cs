using Godot;

public partial class BTShouldEnrage : BTNode
{
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		float angerLevel = blackboard.GetValue<float>("AngerLevel", 0.0f);
		float maxAnger = blackboard.GetValue<float>("MaxAnger", 11.0f);
		
		// Probability based on anger level
		float enrageChance = angerLevel / maxAnger;
		bool shouldEnrage = GD.Randf() < enrageChance * 0.01f;
		
		return shouldEnrage ? BTState.Success : BTState.Failure;
	}
}

using Godot;

public partial class BTStartEvading : BTNode
{
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		int timesSpotted = blackboard.GetValue<int>("TimesSpotted", 0);
		timesSpotted++;
		blackboard.SetValue("TimesSpotted", timesSpotted);
		
		// Every third time spotted, shorter evade time
		float evadeDuration;
		if (timesSpotted % 3 == 0)
		{
			evadeDuration = blackboard.GetValue<float>("ShortEvadeDuration", 5.5f);
		}
		else
		{
			evadeDuration = blackboard.GetValue<float>("NormalEvadeDuration", 12.0f);
		}
		
		blackboard.SetValue("EvadeTimer", evadeDuration);
		
		return BTState.Running;
	}
}

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
			evadeDuration = blackboard.GetValue<float>("ShortEvadeDuration", 11.0f);
		}
		else
		{
			evadeDuration = blackboard.GetValue<float>("NormalEvadeDuration", 24.0f);
		}
		
		blackboard.SetValue("EvadeTimer", evadeDuration);
		
		GD.Print($"Enemy spotted! Evading for {evadeDuration} seconds");
		
		return BTState.Success;
	}
}

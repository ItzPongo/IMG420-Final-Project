using Godot;

public partial class BTUpdateAnger : BTNode
{
	[Export] public float AngerIncreaseRate = 5.0f;
	[Export] public float AngerDecayRate = 0.5f;
	[Export] public float MaxAnger = 11.0f;
	[Export] public bool UseAcceleratingAnger = true;
	
	public override void Initialize(Node agent, Blackboard blackboard)
	{
		base.Initialize(agent, blackboard);
		
		// Initialize anger values if not already set
		if (!blackboard.HasValue("AngerLevel"))
			blackboard.SetValue("AngerLevel", 0.0f);
		if (!blackboard.HasValue("TimeInFlashlight"))
			blackboard.SetValue("TimeInFlashlight", 0.0f);
		if (!blackboard.HasValue("MaxAnger"))
			blackboard.SetValue("MaxAnger", MaxAnger);
	}
	
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		float delta = blackboard.GetValue<float>("Delta", 0.016f);
		float angerLevel = blackboard.GetValue<float>("AngerLevel", 0.0f);
		bool isInFlashlight = blackboard.GetValue<bool>("IsInFlashlight", false);
		float timeInFlashlight = blackboard.GetValue<float>("TimeInFlashlight", 0.0f);
		
		if (isInFlashlight)
		{
			// Track how long enemy has been in the light
			timeInFlashlight += delta;
			
			// Calculate anger increase
			float angerIncrease = AngerIncreaseRate * delta;
			
			if (UseAcceleratingAnger)
			{
				float timeMultiplier = 1.0f + (timeInFlashlight * 0.3f);
				angerIncrease *= timeMultiplier;
			}
			
			angerLevel = Mathf.Min(angerLevel + angerIncrease, MaxAnger);
		}
		else
		{
			// Reset time counter when not in light
			timeInFlashlight = 0.0f;
			
			// Decay anger slowly
			angerLevel = Mathf.Max(angerLevel - AngerDecayRate * delta, 0.0f);
		}
		
		// Update blackboard
		blackboard.SetValue("AngerLevel", angerLevel);
		blackboard.SetValue("TimeInFlashlight", timeInFlashlight);
		
		return BTState.Success;
	}
}

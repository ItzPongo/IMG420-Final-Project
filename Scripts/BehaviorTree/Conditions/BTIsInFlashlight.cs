using Godot;
public partial class BTIsInFlashlight : BTNode
{
	[Export] public float AngerIncreaseAmount = 2.0f;
	[Export] public float AngerIncreaseInterval = 2.0f;
	[Export] public float AngerDecayRate = 0.05f;
	
	private CollisionPolygon2D cachedLightHitbox;
	private Camera2D cachedCamera;
	
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		if (agent is not CharacterBody2D enemy)
			return BTState.Failure;
		
		if (cachedLightHitbox == null || !IsInstanceValid(cachedLightHitbox))
		{
			cachedLightHitbox = agent.GetTree().Root.FindChild("LightHitbox", true, false) as CollisionPolygon2D;
		}
		
		if (cachedLightHitbox == null)
		{
			UpdateAnger(blackboard, false);
			return BTState.Failure;
		}
		
		if (cachedCamera == null || !IsInstanceValid(cachedCamera))
		{
			cachedCamera = enemy.GetViewport().GetCamera2D();
		}
		
		if (cachedCamera == null)
		{
			UpdateAnger(blackboard, false);
			return BTState.Failure;
		}
		
		// Convert enemy world position to screen space
		Vector2 enemyWorldPos = enemy.GlobalPosition;
		Vector2 viewportSize = enemy.GetViewportRect().Size;
		Vector2 enemyScreenPos = enemyWorldPos - cachedCamera.GlobalPosition + viewportSize / 2;
		
		// Get the hitbox transform and convert enemy screen position to local hitbox space
		Transform2D hitboxTransform = cachedLightHitbox.GlobalTransform;
		Vector2 localPos = hitboxTransform.AffineInverse() * enemyScreenPos;
		
		// Check if the point is inside the polygon
		Vector2[] polygon = cachedLightHitbox.Polygon;
		bool isInside = Geometry2D.IsPointInPolygon(localPos, polygon);
		
		// Update blackboard flag
		blackboard.SetValue("IsInFlashlight", isInside);
		
		UpdateAnger(blackboard, isInside);
		
		return isInside ? BTState.Success : BTState.Failure;
	}
	
	private void UpdateAnger(Blackboard blackboard, bool isInFlashlight)
	{
		float delta = blackboard.GetValue<float>("Delta", 0.016f);
		float angerLevel = blackboard.GetValue<float>("AngerLevel", 0.0f);
		float timeInFlashlight = blackboard.GetValue<float>("TimeInFlashlight", 0.0f);
		float angerTimer = blackboard.GetValue<float>("AngerTimer", 0.0f);
		bool hasReceivedFirstAnger = blackboard.GetValue<bool>("HasReceivedFirstAnger", false);
		float maxAnger = blackboard.GetValue<float>("MaxAnger", 11.0f);
		
		if (angerLevel >= maxAnger && !blackboard.GetValue<bool>("IsEnraged", false))
		{
			blackboard.SetValue("IsEnraged", true);
			blackboard.SetValue("EvadeTimer", 0.0f);
		}
		
		// Don't decay anger if enraged
		bool isEnraged = blackboard.GetValue<bool>("IsEnraged", false);
		
		if (isInFlashlight)
		{
			timeInFlashlight += delta;
			
			// First anger increase is instant
			if (!hasReceivedFirstAnger)
			{
				angerLevel = Mathf.Min(angerLevel + AngerIncreaseAmount, maxAnger);
				hasReceivedFirstAnger = true;
				blackboard.SetValue("HasReceivedFirstAnger", true);
			}
			else
			{
				angerTimer += delta;
				
				if (angerTimer >= AngerIncreaseInterval)
				{
					angerLevel = Mathf.Min(angerLevel + AngerIncreaseAmount, maxAnger);
					angerTimer = 0.0f;
				}
			}
		}
		else
		{
			timeInFlashlight = 0.0f;
			angerTimer = 0.0f;
			hasReceivedFirstAnger = false;
			blackboard.SetValue("HasReceivedFirstAnger", false);
			
			// Only decay if not enraged and there's anger to decay
			if (!isEnraged && angerLevel > 0.0f)
			{
				angerLevel = Mathf.Max(angerLevel - AngerDecayRate * delta, 0.0f);
			}
		}
		
		blackboard.SetValue("AngerLevel", angerLevel);
		blackboard.SetValue("TimeInFlashlight", timeInFlashlight);
		blackboard.SetValue("AngerTimer", angerTimer);
	}
}

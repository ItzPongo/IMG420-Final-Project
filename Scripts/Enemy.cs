using Godot;

public partial class Enemy : CharacterBody2D
{
	[ExportGroup("Stats")]
	[Export] public float SneakSpeed = 30f;
	[Export] public float EvadeSpeed = 100f;
	[Export] public float EnragedSpeed = 150f;
	
	[ExportGroup("Detection")]
	[Export] public float KillRange = 30f;
	[Export] public float SafeDistance = 200f;
	
	[ExportGroup("Anger System")]
	[Export] public float AngerIncreaseRate = 2.0f;
	[Export] public float AngerDecayRate = 0.5f;
	[Export] public float MaxAnger = 11.0f;
	
	[ExportGroup("Evade Timing")]
	[Export] public float NormalEvadeDuration = 24.0f;
	[Export] public float ShortEvadeDuration = 11.0f;
	
	private Blackboard blackboard;
	private BTNode behaviorTreeRoot;
	private Label stateLabel;
	private CharacterBody2D player;
	private AnimatedSprite2D sprite;
	
	public override void _Ready()
	{
		blackboard = new Blackboard();
		
		InitializeBlackboard();
		
		stateLabel = GetNodeOrNull<Label>("StateLabel");
		sprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		behaviorTreeRoot = GetNodeOrNull<BTNode>("BehaviorTree");
		
		CallDeferred(nameof(FindPlayer));
		
		if (behaviorTreeRoot != null)
		{
			behaviorTreeRoot.Initialize(this, blackboard);
		}
	}
	
	private void FindPlayer()
	{
		// Look for the Player CharacterBody2D node
		var players = GetTree().GetNodesInGroup("player");
		if (players.Count > 0)
		{
			player = players[0] as CharacterBody2D;
			GD.Print($"Enemy: Found player via group: {player.Name}");
		}
		else
		{
			// Fallback: search by name
			player = GetTree().Root.FindChild("Player", true, false) as CharacterBody2D;
			if (player != null)
			{
				GD.Print($"Enemy: Found player via FindChild: {player.Name}");
			}
		}
		
		if (player == null)
		{
			GD.PrintErr("Enemy: Could not find Player node!");
		}
		blackboard.SetValue("Player", player);
	}
	
	private void InitializeBlackboard()
	{
		blackboard.SetValue("AngerLevel", 0.0f);
		blackboard.SetValue("MaxAnger", MaxAnger);
		blackboard.SetValue("AngerIncreaseRate", AngerIncreaseRate);
		blackboard.SetValue("AngerDecayRate", AngerDecayRate);
		blackboard.SetValue("IsInFlashlight", false);
		blackboard.SetValue("TimeInFlashlight", 0.0f);
		blackboard.SetValue("TimesSpotted", 0);
		blackboard.SetValue("EvadeTimer", 0.0f);
		blackboard.SetValue("NormalEvadeDuration", NormalEvadeDuration);
		blackboard.SetValue("ShortEvadeDuration", ShortEvadeDuration);
		blackboard.SetValue("CurrentState", "Sneaking");
		blackboard.SetValue("KillRange", KillRange);
		blackboard.SetValue("SneakSpeed", SneakSpeed);
		blackboard.SetValue("EvadeSpeed", EvadeSpeed);
		blackboard.SetValue("EnragedSpeed", EnragedSpeed);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		blackboard.SetValue("Delta", (float)delta);
		
		UpdateAnger((float)delta);
		
		if (behaviorTreeRoot != null)
		{
			behaviorTreeRoot.Tick(this, blackboard);
		}
		
		UpdateUI();
		UpdateAnimation();
	}
	
	private void UpdateAnger(float delta)
	{
		float angerLevel = blackboard.GetValue<float>("AngerLevel", 0.0f);
		bool isInFlashlight = blackboard.GetValue<bool>("IsInFlashlight", false);
		float timeInFlashlight = blackboard.GetValue<float>("TimeInFlashlight", 0.0f);
		float increaseRate = blackboard.GetValue<float>("AngerIncreaseRate", 2.0f);
		float decayRate = blackboard.GetValue<float>("AngerDecayRate", 0.5f);
		float maxAnger = blackboard.GetValue<float>("MaxAnger", 11.0f);
		
		if (isInFlashlight)
		{
			timeInFlashlight += delta;
			angerLevel = Mathf.Min(angerLevel + increaseRate * delta, maxAnger);
		}
		else
		{
			timeInFlashlight = 0.0f;
			angerLevel = Mathf.Max(angerLevel - decayRate * delta, 0.0f);
		}
		
		blackboard.SetValue("AngerLevel", angerLevel);
		blackboard.SetValue("TimeInFlashlight", timeInFlashlight);
	}
	
	private void UpdateUI()
	{
		if (stateLabel != null)
		{
			string state = blackboard.GetValue<string>("CurrentState", "Unknown");
			float anger = blackboard.GetValue<float>("AngerLevel", 0.0f);
			stateLabel.Text = $"{state}\nAnger: {anger:F1}/{MaxAnger:F1}";
		}
	}
	
	private void UpdateAnimation()
	{
		if (sprite == null || sprite.SpriteFrames == null) return;
		
		// Set animation speed
		sprite.SpeedScale = 1.0f; // This will play at the FPS set in the SpriteFrames (5 FPS)
		
		if (Velocity.Length() > 0)
		{
			// Flip sprite based on horizontal movement
			if (Velocity.X != 0)
			{
				sprite.FlipH = Velocity.X < 0;
			}
			
			// Play walk animation
			if (sprite.SpriteFrames.HasAnimation("walk"))
				sprite.Play("walk");
		}
		else
		{
			// Stop on first frame when idle
			if (sprite.SpriteFrames.HasAnimation("walk"))
			{
				sprite.Animation = "walk";
				sprite.Frame = 0;
				sprite.Stop();
			}
		}
	}
	
	// Called by flashlight detection system
	public void EnterFlashlight()
	{
		blackboard.SetValue("IsInFlashlight", true);
	}
	
	public void ExitFlashlight()
	{
		blackboard.SetValue("IsInFlashlight", false);
	}
}

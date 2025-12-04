using Godot;

public partial class Enemy : CharacterBody2D
{
	[ExportGroup("Stats")]
	[Export] public float SneakSpeed = 150f;
	[Export] public float EvadeSpeed = 100f;
	[Export] public float EnragedSpeed = 230f;
	
	[ExportGroup("Detection")]
	[Export] public float KillRange = 30f;
	[Export] public float SafeDistance = 200f;
	
	[ExportGroup("Anger System")]
	[Export] public float AngerIncreaseRate = 8.0f;
	[Export] public float AngerDecayRate = 0.5f;
	[Export] public float MaxAnger = 11.0f;
	[Export] public bool UseAcceleratingAnger = true;
	
	[ExportGroup("Evade Timing")]
	[Export] public float NormalEvadeDuration = 12.0f;
	[Export] public float ShortEvadeDuration = 5.5f;
	
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
		var players = GetTree().GetNodesInGroup("player");
		if (players.Count > 0)
		{
			player = players[0] as CharacterBody2D;
		}
		else
		{
			player = GetTree().Root.FindChild("Player", true, false) as CharacterBody2D;
			if (player != null)
			{
			}
		}
		
		if (player == null)
		{
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
		blackboard.SetValue("IsEnraged", false);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		blackboard.SetValue("Delta", (float)delta);
		
		// Stop ticking if player is dead
		bool playerIsDead = blackboard.GetValue<bool>("PlayerIsDead", false);
		if (playerIsDead)
		{
			Velocity = Vector2.Zero;
			return;
		}
		
		DecayAnger((float)delta);
		
		if (behaviorTreeRoot != null)
		{
			behaviorTreeRoot.Tick(this, blackboard);
		}
		
		UpdateUI();
		UpdateAnimation();
	}

	private void DecayAnger(float delta)
	{
		// Don't decay if enraged
		bool isEnraged = blackboard.GetValue<bool>("IsEnraged", false);
		if (isEnraged)
			return;
			
		bool isInFlashlight = blackboard.GetValue<bool>("IsInFlashlight", false);
		if (!isInFlashlight)
		{
			float angerLevel = blackboard.GetValue<float>("AngerLevel", 0.0f);
			if (angerLevel > 0.0f)
			{
				angerLevel = Mathf.Max(angerLevel - 0.05f * delta, 0.0f);
				blackboard.SetValue("AngerLevel", angerLevel);
			}
		}
	}
	
	private void UpdateUI()
	{
		if (stateLabel != null)
		{
			string state = blackboard.GetValue<string>("CurrentState", "Unknown");
			float anger = blackboard.GetValue<float>("AngerLevel", 0.0f);
			bool isEnraged = blackboard.GetValue<bool>("IsEnraged", false);
			string enragedText = isEnraged ? " [ENRAGED]" : "";
			stateLabel.Text = $"{state}{enragedText}\nAnger: {anger:F1}/{MaxAnger:F1}";
		}
	}
	
	private void UpdateAnimation()
	{
		if (sprite == null || sprite.SpriteFrames == null) return;
		
		sprite.SpeedScale = 1.0f;
		
		if (Velocity.Length() > 0)
		{
			if (Velocity.X != 0)
			{
				sprite.FlipH = Velocity.X < 0;
			}
			
			if (sprite.SpriteFrames.HasAnimation("walk"))
				sprite.Play("walk");
		}
		else
		{
			if (sprite.SpriteFrames.HasAnimation("walk"))
			{
				sprite.Animation = "walk";
				sprite.Frame = 0;
				sprite.Stop();
			}
		}
	}
	
	public void EnterFlashlight()
	{
		blackboard.SetValue("IsInFlashlight", true);
	}
	
	public void ExitFlashlight()
	{
		blackboard.SetValue("IsInFlashlight", false);
	}
}

  using Godot;

	public partial class Player : CharacterBody2D
	{
		[Export]
		public float Speed { get; set; } = 200.0f;
		
		public float CurrentBattery {get; private set; }
		[Export] public float MaxBattery = 100f;
		[Export] public float DrainPerSecond = 5f;
		[Export] public NodePath BatteryBarPath;
		
		private TextureProgressBar _batteryBar;
		
		public override void _Ready()
		{
			CurrentBattery = MaxBattery;
			
			if (BatteryBarPath != null && !BatteryBarPath.IsEmpty)
			{
				_batteryBar = GetNode<TextureProgressBar>(BatteryBarPath);
				_batteryBar.MaxValue = MaxBattery;
				_batteryBar.Value = CurrentBattery;
			}
		}

		public override void _PhysicsProcess(double delta)
		{
			Vector2 inputDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
			Velocity = inputDirection * Speed;
			MoveAndSlide();
			
			UpdateBattery((float)delta);
		}
		
		private void UpdateBattery(float delta)
		{
			if (CurrentBattery <= 0)
			{
				CurrentBattery = 0;
				return;
			}
			
			CurrentBattery -= DrainPerSecond * delta;
			CurrentBattery = Mathf.Clamp(CurrentBattery, 0, MaxBattery);
			
			if (_batteryBar != null)
			{
				_batteryBar.Value = CurrentBattery;
			}
		}
		
		public void AddBattery(float amount)
		{
			CurrentBattery = Mathf.Clamp(CurrentBattery + amount, 0, MaxBattery);
			
			if (_batteryBar != null)
			{
				_batteryBar.Value = CurrentBattery;
				GD.Print($"Battery recharged by {amount}. Current: {CurrentBattery}/{MaxBattery}");
			}
		}
	}

using Godot;

public partial class Hud : CanvasLayer
{
	[Export] public Label ScoreLabel;
	[Export] public Label BatteryLabel;

	[Export] public Timer ScoreTimer;
	[Export] public Timer BatteryTimer;
	
	[Signal] public delegate void BatteryUpdateEventHandler();

	private int elapsedSeconds = 0;
	private float batteryPercent = 10000f;
	private float batteryText = 0f;
	private float batteryStep = 1f;
	private float batteryMin = 0f;
	private float batteryMax = 0f;

	public override void _Ready()
	{
		batteryMax = batteryPercent;
		
		UpdateScoreLabel();
		UpdateBattery();
	}

	private void OnScoreTimerTimeout()
	{
		elapsedSeconds++;

		UpdateScoreLabel();
	}

	private void OnBatteryTimerTimeout()
	{
		if (batteryPercent > batteryMin)
		{
			batteryPercent -= batteryStep;
		}

		UpdateBattery();
	}

	private void UpdateScoreLabel()
	{
		int minutes = elapsedSeconds / 60;
		int seconds = elapsedSeconds % 60;

		ScoreLabel.Text = $"{minutes:D1}:{seconds:D2}";
	}

	private void UpdateBattery()
	{
		batteryText = batteryPercent / 100f;
		
		BatteryLabel.Text = $"{Mathf.RoundToInt(batteryText)}%";
		
		EmitSignal(SignalName.BatteryUpdate, batteryPercent);
	}
	
	private void OnBatteryPickup(float batteryValue)
	{
		float batteryIncrease = batteryPercent + batteryValue;
		if (batteryIncrease > batteryMax)
		{
			batteryPercent = batteryMax;
		}
		else
		{
			batteryPercent += batteryValue;
		}
	}
}

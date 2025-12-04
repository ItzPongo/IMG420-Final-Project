using Godot;
public partial class Hud : CanvasLayer
{
	[Export] public Label ScoreLabel;
	[Export] public Timer ScoreTimer;
	private int elapsedSeconds = 0;
	private int minutes = 0;
	private int seconds = 0;
	
	[Export] Control GameOver;
	[Export] Label DescriptionText;
	
	[Signal] public delegate void BatteryUpdateEventHandler(float batteryPercent);
	
	public override void _Ready()
	{
		UpdateScoreLabel();
		GameOver.Visible = false;
		
		ProcessMode = ProcessModeEnum.Always;
		
		// Connect to player death signal
		CallDeferred(nameof(ConnectToPlayer));
	}
	
	private void ConnectToPlayer()
	{
		var players = GetTree().GetNodesInGroup("player");
		Player player = null;
		
		if (players.Count > 0)
		{
			player = players[0] as Player;
		}
		else
		{
			player = GetTree().Root.FindChild("Player", true, false) as Player;
			if (player != null)
			{
			}
		}
		
		if (player != null)
		{
			player.PlayerDeath += OnPlayerDeath;
		}
	}
	
	private void OnScoreTimerTimeout()
	{
		elapsedSeconds++;
		UpdateScoreLabel();
	}
	
	private void UpdateScoreLabel()
	{
		minutes = elapsedSeconds / 60;
		seconds = elapsedSeconds % 60;
		ScoreLabel.Text = $"{minutes:D1}:{seconds:D2}";
	}
	
	private void OnPlayerDeath()
	{
		ScoreLabel.Visible = false;
		GameOver.Visible = true;
		DescriptionText.Text = $"You survived for {minutes:D1}:{seconds:D2}!";
		
		// Stop the score timer
		if (ScoreTimer != null)
		{
			ScoreTimer.Stop();
		}
		
		// Pause the game
		GetTree().Paused = true;
	}
	
	private void OnRestartButtonPressed()
	{
		// Unpause before changing scene
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://Scenes/main.tscn");
	}
}

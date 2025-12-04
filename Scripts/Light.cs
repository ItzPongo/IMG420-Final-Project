using Godot;
public partial class Light : Sprite2D
{
	[Export] public Node2D Pivot;
	[Export] public CollisionPolygon2D LightHitbox;
	[Export] public float Radius = 150f;
	
	private float batteryDecimal = 1f;
	
	private float baseRadius;
	private Vector2 baseScale;
	private Player player;
	private Camera2D camera;
	
	public override void _Ready()
	{
		baseRadius = Radius;
		baseScale = Scale;
		
		// Connect to player
		CallDeferred(nameof(ConnectToPlayer));
	}
	
	private void ConnectToPlayer()
	{
		var players = GetTree().GetNodesInGroup("player");
		if (players.Count > 0 && players[0] is Player foundPlayer)
		{
			player = foundPlayer;
			
			// Find the camera
			camera = player.GetViewport().GetCamera2D();
		}
	}
	
	public override void _Process(double delta)
	{
		if (Pivot == null || player == null || camera == null)
			return;
		
		// Update battery-based scaling
		float newBatteryDecimal = Mathf.Clamp(player.CurrentBattery / player.MaxBattery, 0.1f, 1f);
		if (Mathf.Abs(newBatteryDecimal - batteryDecimal) > 0.01f)
		{
			batteryDecimal = newBatteryDecimal;
			Radius = baseRadius * batteryDecimal;
			Scale = baseScale * batteryDecimal;
		}
		
		// Convert player position from world space to canvas space
		Vector2 playerScreenPos = player.GlobalPosition - camera.GlobalPosition + GetViewportRect().Size / 2;
		
		Pivot.Position = playerScreenPos;
		
		Vector2 pivotPos = Pivot.Position;
		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector2 dir = mousePos - pivotPos;
		
		if (dir.LengthSquared() < 0.00001f)
			return;
		
		float angle = dir.Angle();
		Position = pivotPos + new Vector2(Radius, 0).Rotated(angle);
		Rotation = angle;
		
		if (LightHitbox != null)
		{
			LightHitbox.Position = Position;
			LightHitbox.Rotation = Rotation;
			LightHitbox.Scale = Scale * 4.0f;
		}
	}
	
	// This method converts screen space to world space for enemy detection
	public Vector2 GetWorldPosition()
	{
		if (camera == null)
			return Vector2.Zero;
			
		// Convert from screen space back to world space
		return Position + camera.GlobalPosition - GetViewportRect().Size / 2;
	}
	
	public bool IsInLightCone(Vector2 targetPosition, float coneAngle = 45f)
	{
		if (player == null)
			return false;
			
		Vector2 pivotPos = player.GlobalPosition;
		Vector2 toTarget = targetPosition - pivotPos;
		float distance = toTarget.Length();
		
		float effectiveRadius = Radius * batteryDecimal;
		
		if (distance > effectiveRadius)
			return false;
		
		float angleToTarget = toTarget.Angle();
		float angleDiff = Mathf.Abs(Mathf.AngleDifference(angleToTarget, Rotation));
		float maxAngle = Mathf.DegToRad(coneAngle / 2f);
		
		return angleDiff <= maxAngle;
	}
}

using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace FirstGame.scripts;

public partial class Main : Node
{
	[Export]
	public PackedScene EnemyTemplate { get; set; }

	private int _score;
	
	private Marker2D _startingPosition;
	private Timer _scoreTimer;
	private Timer _mobTimer;
	private Timer _startTimer;
	private Player _player;
	private CanvasLayerScript _hud;

	private AudioStreamPlayer _mainSoundPlayer;
	private AudioStreamPlayer _deathPlayer;

	public override void _Ready()
	{
		_startingPosition = GetNode<Marker2D>("StartingPositionMarker2D");
		_scoreTimer = GetNode<Timer>("ScoreTimer");
		_mobTimer = GetNode<Timer>("MobTimer");
		_startTimer = GetNode<Timer>("StartTimer");
		_player = GetNode<Player>("Player");

		_mainSoundPlayer = GetNode<AudioStreamPlayer>("MainSoundPlayer");
		_deathPlayer = GetNode<AudioStreamPlayer>("DeathPlayer");

		_hud = GetNode<CanvasLayerScript>("CanvasLayer");
		
		_hud.GameStart += HudOnGameStart;
	}

	private void HudOnGameStart() => NewGame();

	private void _OnStartTimerTimeout()
	{
		_score = 0;
		SpawnPlayer();
	}
	
	private void _OnScoreTimerTimeout()
	{	
		_score++;
		
		_hud.UpdateScore(_score);
	}

	private void _onMobTimerTimeout() => SpawnMob();

	private void _OnPlayerHit() => GameOver();

	private void SpawnPlayer()
	{
		_player.Start(_startingPosition.Position);
		_mobTimer.Start();
	}

	private void SpawnMob()
	{
		// Create a new instance of the Mob scene.
		var mob = EnemyTemplate.Instantiate<Enemy>();
		
		// Choose a random location on Path2D.
		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.ProgressRatio = Math.Max(0.001f, GD.Randf());
		
		// Set the mob's direction perpendicular to the path direction.
		var direction = mobSpawnLocation.RotationDegrees + 90;
		
		// Set the mob's position to a random location.
		mob.Position = mobSpawnLocation.Position;
		
		// Add some randomness to the direction.
		direction += GD.RandRange(-45, 45);
		mob.RotationDegrees = direction;
		
		// Choose the velocity.
		var velocity = new Vector2((float) GD.RandRange(150.0, 350.0), 0);
		
		mob.LinearVelocity = velocity.Rotated(Mathf.DegToRad(direction));
		
		// Spawn the mob by adding it to the Main scene.
		AddChild(mob);
	}

	private async Task GameOver()
	{
		_scoreTimer.Stop();
		_mobTimer.Stop();
		_mainSoundPlayer.Stop();
		
		_deathPlayer.Play();
		
		GetTree().CallGroup("enemies", Node.MethodName.QueueFree);
		foreach (var node in GetChildren().Where(n => n is Enemy))
		{
			node.QueueFree();
			RemoveChild(node);
		}

		await _hud.ShowGameOver();
	}

	private void NewGame()
	{
		_hud.UpdateScore(_score);
		_hud.ShowMessage("Get Ready!");

		_mainSoundPlayer.Play();
		
		_scoreTimer.Start();
		_startTimer.Start();
	}
}

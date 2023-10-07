using System.Threading.Tasks;
using Godot;

namespace FirstGame.scripts;

public partial class CanvasLayerScript : CanvasLayer
{
	private Label _messageLabel;
	private Timer _messageTimer;
	private Button _startButton;
	private Label _scoreLabel;

	[Signal]
	public delegate void GameStartEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_messageLabel = GetNode<Label>("MessageLabel");
		_messageTimer = GetNode<Timer>("MessageTimer");
		_startButton = GetNode<Button>("StartButton");
		_scoreLabel = GetNode<Label>("ScoreLabel");
	}

	public void ShowMessage(string message, bool withTimer = true)
	{
		_messageLabel.Text = message;
		_messageLabel.Show();

		if (withTimer)
		{
			_messageTimer.Start();
		}
	}

	public async Task ShowGameOver()
	{
		ShowMessage("Game Over");

		await ToSignal(_messageTimer, Timer.SignalName.Timeout);
		
		ShowMessage("Dodge the\nCreeps!", false);

		await ToSignal(GetTree().CreateTimer(1), Timer.SignalName.Timeout);
		_startButton.Show();
	}

	public void UpdateScore(int score) => _scoreLabel.Text = $"Score: {score}";

	private void _OnMessageTimerTimeout() => _messageLabel.Hide();
	
	private void _onStartButtonPressed()
	{
		GD.Print("Start button pressed");
		_startButton.Hide();
		_messageLabel.Hide();
		EmitSignal(SignalName.GameStart);
	}
}

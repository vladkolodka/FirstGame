using Godot;

public partial class Player : Area2D
{
	[Export] 
	public int Speed { get; set; } = 400;

	private Vector2 _screenSize;
	private AnimatedSprite2D _animatedSprite;
	private CollisionShape2D _collisionShape;

	[Signal]
	public delegate void HitEventHandler();
	
	public override void _Ready()
	{
		_screenSize = GetViewportRect().Size;
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		
		// Hide();
	}

	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero;

		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y = -1;
		}

		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y = 1;
		}
		
		if (Input.IsActionPressed("move_left"))
		{
			velocity.X = -1;
		}
		
		if (Input.IsActionPressed("move_right"))
		{
			velocity.X = 1;
		}

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;

			if (velocity.X != 0)
			{
				_animatedSprite.Animation = "walk";
				_animatedSprite.FlipV = false;
				_animatedSprite.FlipH = velocity.X < 0;
			}
			else if (velocity.Y != 0)
			{
				_animatedSprite.Animation = "up";
				_animatedSprite.FlipV = velocity.Y > 0;
			}
			
			_animatedSprite.Play();
		}
		else
		{
			_animatedSprite.Stop();
		}

		Position = (Position + velocity * (float) delta)
			.Clamp(Vector2.Zero, _screenSize);
	}

	public void Start(Vector2 position)
	{
		Position = position;
		_collisionShape.Disabled = false;
		Show();
	}
	
	private void _OnBodyEntered(Node2D body)
	{
		Hide();
		EmitSignal(SignalName.Hit);
		
		// _collisionShape.Disabled = true; // TODO try
		_collisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}
}



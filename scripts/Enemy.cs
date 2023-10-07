using Godot;

namespace FirstGame.scripts;

public partial class Enemy : RigidBody2D
{
	private AnimatedSprite2D _animatedSprite;

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		var animations = _animatedSprite.SpriteFrames.GetAnimationNames();
		_animatedSprite.Play(animations[GD.Randi() % animations.Length]);
	}
	
	private void _OnVisibleOnScreenNotifier2dScreenExited()
	{
		QueueFree();
	}
}

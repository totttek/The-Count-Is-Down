using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

public partial class Kmicic : CharacterBody2D
{
	public bool isDeathScene = false;
	private const float speed_normalizer = 100.0f;
	private const float speed_damper = 1500.0f;
	[Export] private float Speed = 3.0f;
	private float direction = 0.0f;

	[Export] public AnimatedSprite2D Animation;

	public override void _Process(double delta)
	{
		if (direction != 0 && !isDeathScene)
		{
			Animation.Play("walk");
			if (direction == -1f)
				Animation.FlipH = true;
			else
				Animation.FlipH = false;
		}
		else if (!isDeathScene)
		{
			Animation.Stop();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		if (direction != 0)
		{
			velocity.X = direction * Speed * speed_normalizer;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, speed_damper * (float)delta);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
	{
		direction = Input.GetAxis("LEFT", "RIGHT");
	}

	public void SetDirection(float newDirection)
	{
		direction = newDirection;
	}
}

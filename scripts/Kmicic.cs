using Godot;
using System;

public partial class Kmicic : CharacterBody2D
{
	private const float speed_normalizer = 100.0f;
	private const float speed_damper = 1500.0f;
	[Export] private float Speed = 3.0f;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		float direction = Input.GetAxis("LEFT", "RIGHT");
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
}

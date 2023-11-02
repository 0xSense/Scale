using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float maxSpeed = 400f;
	public const float jumpVelocity = -400.0f;
	public const float accelerationRate = 100 * 0.4f;
	public const int jumpBufferTimer = 15; // 1/4s = 15/60
	public int jumpBufferCounter;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;


		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity.Y += gravity * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = jumpVelocity;
		}
		if (jumpBufferCounter > 0)
		{
			jumpBufferCounter -= 1;
		}
		if (jumpBufferCounter > 0 && IsOnFloor())
		{
			jumpBufferCounter = 0;
		}

		// Movement Based on the X axis
		if (Input.IsActionPressed("ui_left"))
		{
			velocity.X -= accelerationRate;
		}

		if (Input.IsActionPressed("ui_right"))
		{
			velocity.X += accelerationRate;
		}

		if (!Input.IsActionPressed("ui_left") && !Input.IsActionPressed("ui_right"))
		{
			velocity.X = (float)Mathf.Lerp(velocity.X, 0, 0.257);
		}


		velocity.X = Mathf.Clamp(velocity.X, -maxSpeed, maxSpeed);
		GD.Print(velocity.X);
		Velocity = velocity;
		MoveAndSlide();

	}
}

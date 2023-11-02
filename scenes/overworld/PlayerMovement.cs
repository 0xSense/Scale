using Godot;
using System;

public partial class PlayerMovement : CharacterBody2D
{
	public const float maxSpeed = 300.0f;
	public float currentSpeed;
	public float speedDiff;
	public const float jumpVelocity = -400.0f;
	public const float accelerationRate = 0.8f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = jumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			speedDiff = maxSpeed - Math.Abs(Velocity.X);
			currentSpeed = speedDiff * accelerationRate;
			velocity.X = direction.X * currentSpeed;
			GD.Print(Velocity.X);
			GD.Print(speedDiff);
			GD.Print(currentSpeed);
			GD.Print(velocity.X);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, currentSpeed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}

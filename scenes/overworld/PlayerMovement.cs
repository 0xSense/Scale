using Godot;
using System;

public partial class PlayerMovement : CharacterBody2D
{
	public const float maxSpeed = 432.130f;
	public float currentSpeed;
	public float speedDiff;
	public const float jumpVelocity = -400.0f;
	public const float accelerationRate = 40f;

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
		if (Input.IsActionPressed("ui_left")) {
			//speedDiff = maxSpeed - Math.Abs(Velocity.X); 
			velocity.X -= accelerationRate;
		}
		else if (Input.IsActionPressed("ui_right")) {
			//speedDiff = maxSpeed - Math.Abs(Velocity.X); 
			velocity.X += accelerationRate;
		}
		else 
		{
			velocity.X = (float)Mathf.Lerp(velocity.X, 0, 0.257);
		}

		/*
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			speedDiff = maxSpeed - Math.Abs(Velocity.X); 
			currentSpeed = speedDiff * accelerationRate;
			velocity.X = direction.X * currentSpeed;
		}
		else
		{
			speedDiff = maxSpeed - Math.Abs(Velocity.X); 
			currentSpeed = speedDiff * accelerationRate;
			velocity.X = Mathf.MoveToward(Velocity.X, 0, currentSpeed);
		}
*/		
		velocity.X = Mathf.Clamp(velocity.X, -maxSpeed, maxSpeed);
		Velocity = velocity;
		MoveAndSlide();
		
	}
}

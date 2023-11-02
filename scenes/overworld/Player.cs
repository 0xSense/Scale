using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float maxSpeed = 432.130f;
	public float speed;
	public float speedDiff;
	public const float jumpVelocity = -400.0f;
	public const float accelerationRate = 0.4f;
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
		if (jumpBufferCounter > 0 && IsOnFloor()){
			
			jumpBufferCounter = 0;
		}

		// Movement Based on the X axsis
		if (Input.IsActionPressed("ui_left")) 
		{
			speedDiff = maxSpeed - Math.Abs(velocity.X);
			velocity.X -= speedDiff * accelerationRate;
		}
		else 
		{
			velocity.X = (float)Mathf.Lerp(velocity.X, 0, 0.257);
		}
		
		if (Input.IsActionPressed("ui_right")) {
			speedDiff = maxSpeed - Math.Abs(velocity.X);
			velocity.X += speedDiff * accelerationRate;
		}
		else 
		{
			velocity.X = (float)Mathf.Lerp(velocity.X, 0, 0.257);
		}

		velocity.X = Mathf.Clamp(velocity.X, -maxSpeed, maxSpeed);
		GD.Print(velocity.X);
		Velocity = velocity;
		MoveAndSlide();
		
	}
}

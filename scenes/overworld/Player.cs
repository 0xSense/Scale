using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public Vector2 collisionShapeSize;
	public const float maxSpeed = 400f;
	public const float maxWalkSpeed = 200f;
	public const float jumpVelocity = -400.0f;
	public float speed = 100;
	public float accelerationRate = 0.4f;
	public const int jumpBufferTimer = 15; // 1/4s = 15/60
	public int jumpBufferCounter;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	public override void _Ready()
    {
        collisionShapeSize = GetNode<CollisionShape2D>("./CollisionShape2D").Scale;
    }

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
			velocity.X -= speed * accelerationRate;
		}

		if (Input.IsActionPressed("ui_right"))
		{
			velocity.X += speed * accelerationRate;
		}


		if (!Input.IsActionPressed("ui_left") && !Input.IsActionPressed("ui_right"))
		{
			velocity.X = (float)Mathf.Lerp(velocity.X, 0, 0.257);
		}

		if (Input.IsActionPressed("ui_sprint"))
		{
			velocity.X = Mathf.Clamp(velocity.X, -maxSpeed, maxSpeed);
		}
		else
		{
			velocity.X = Mathf.Clamp(velocity.X, -maxWalkSpeed, maxWalkSpeed);
		}

		GD.Print(velocity.X);
		Velocity = velocity;
		MoveAndSlide();

		// Crounching
		if (Input.IsActionPressed("ui_crouch"))
		{
			GetNode<CollisionShape2D>("./CollisionShape2D").Scale = new Vector2(collisionShapeSize[0], collisionShapeSize[1] / 2);
		} 
		else
		{
			GetNode<CollisionShape2D>("./CollisionShape2D").Scale = collisionShapeSize;
		} 
	}
}

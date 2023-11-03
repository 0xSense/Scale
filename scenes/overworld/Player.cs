using Godot;
using System;
using System.Data;

/*

Player movement:
 - Walk left and right
 - Jump
  * Gravity increases on descent
 - Crouching (ask Perplexer why this needs to be a thing)
 - Sprinting (ask Perplexer why this needs to be a thing)

*/

public enum State
{
	GROUNDED,
	AIRBORNE
}

public partial class Player : CharacterBody2D
{
	[Export] private float _walkSpeed = 1000f;
	[Export] private float _jumpTime; // Time spent in upward acceleration
	[Export] private float _jumpSpeed = 400f; // Speed player moves upward while jumping
	[Export] private float _gravityFallMultiplier = 2f;
	[Export] public float _accelerationStrength = 0.09f;
	private float _gravityDefault;

	private State _state;

	public override void _Ready()
	{
		_gravityDefault = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
		_state = State.GROUNDED;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		float fDelta = (float)delta;

		Vector2 movementInput = GetMovementInput();
		Vector2 vel = Velocity;
		vel.X = (float)Mathf.Lerp(vel.X, 0, 0.257);

		if (IsOnFloor())
			_state = State.GROUNDED;
		else
			_state = State.AIRBORNE;

		float accelerationRate;
		// Continuing in current direction
		if ((movementInput.X > 0) == (Velocity.X > 0))
		{
			accelerationRate = (_walkSpeed - Mathf.Abs(Velocity.X))/_walkSpeed;
		}
		else
		{
			accelerationRate = 2f;
		}
		switch (_state)
		{
			case State.GROUNDED:				
				vel += movementInput * accelerationRate * _walkSpeed * _accelerationStrength;
				if (GetJumpInput())
				{
					vel.Y = -_jumpSpeed;
				}
				break;
			case State.AIRBORNE:
				vel.X += movementInput.X * accelerationRate * _walkSpeed * _accelerationStrength;
				
				if (vel.Y < 0)
					vel.Y += _gravityDefault * fDelta;
				else
					vel.Y += _gravityDefault * _gravityFallMultiplier * fDelta;

				break;
		}

		Velocity = vel;

		MoveAndSlide();
	}

	private Vector2 GetMovementInput()
	{
		Vector2 movement = new();

		movement.X = Input.GetAxis("ui_left", "ui_right");

		return movement;
	}

	private bool GetJumpInput()
	{
		return Input.IsActionJustPressed("ui_select");
	}

}
namespace Overworld;

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
	[Export] private float _walkSpeed = 500f;
	[Export] private float _sprintSpeed = 1700f;
	[Export] private float _jumpTime; // Time spent in upward acceleration
	[Export] private float _jumpSpeed = 400f; // Speed player moves upward while jumping
	[Export] private float _gravityFallMultiplier = 2f;
	[Export] private float _accelerationStrength = 0.09f;
	[Export] private float _coyoteBuffer = 0.5f; // In seconds
	private float _coyoteTimer;
	private float _gravityDefault;

	private State _state;

	public override void _Ready()
	{
		_gravityDefault = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
		_state = State.GROUNDED;
		_coyoteTimer = _coyoteBuffer;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		float fDelta = (float)delta;

		Vector2 movementInput = GetMovementInput();
		Vector2 vel = Velocity;

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
				_coyoteTimer = _coyoteBuffer;
				vel.X = (float)Mathf.Lerp(vel.X, 0, 0.157);
				if (IsSprinting())
				{
					vel += movementInput * accelerationRate * _sprintSpeed * _accelerationStrength;
				}
				else
				{
					vel += movementInput * accelerationRate * _walkSpeed * _accelerationStrength;
				}
				if (GetJumpInput())
				{
					vel.Y = -_jumpSpeed;
					_coyoteTimer = 0f;
				}
				break;

			case State.AIRBORNE:
				_coyoteTimer -= fDelta;

				if (GetJumpInput() && _coyoteTimer >= 0)
				{
					vel.Y = -_jumpSpeed;
					_coyoteTimer = 0f;
				}

				vel.X = (float)Mathf.Lerp(vel.X, 0, 0.075);
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

	private bool IsSprinting()
	{
		return Input.IsActionPressed("ui_sprint");
	}

}
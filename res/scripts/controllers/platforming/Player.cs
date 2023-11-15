namespace Overworld;

using Godot;
using System;
using System.ComponentModel;
using System.Data;
using System.Numerics;

/*

Player movement:
 - Walk left and right
 - Jump
  * Gravity increases on descent
 - Crouching (ask Perplexer why this needs to be a thing)
 - Sprinting (ask Perplexer why this needs to be a thing)
	When input is "right" character sprite should face right
		
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
	[Export] private float _jumpSpeed = 500f; // Speed player moves upward while jumping
	[Export] private float _gravityFallMultiplier = 2f;
	[Export] private float _accelerationStrength = 0.09f;
	[Export] private float _coyoteBuffer = 0.5f; // In seconds

	private float _coyoteTimer;
	private float _gravityDefault;
	private State _state;
	private AnimatedSprite2D _playerSprite;
	private CharacterBody2D _charcterBody;

	public override void _Ready()
	{
		_gravityDefault = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
		_state = State.GROUNDED;
		_coyoteTimer = _coyoteBuffer;
		// _playerSprite = GetNode<Sprite2D>("PlayerSkin");
		_playerSprite = GetNode<AnimatedSprite2D>("PlayerAnimation");
	}

	public override void _Process(double delta)
	{
		ChangePlayerOrientation();

		if (IsIdle())
		{
			_playerSprite.Play("idle");
		}
		else if (IsSprinting())
		{
			_playerSprite.Play("sprint");
		}
		else if (IsJumping())
		{
			_playerSprite.Play("jump");
		}
		else if (IsWalking())
		{
			_playerSprite.Play("walk");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		float fDelta = (float)delta;

		Godot.Vector2 movementInput = GetMovementInput();

		Godot.Vector2 vel = Velocity;

		if (IsOnFloor())
		{
			_state = State.GROUNDED;
		}
		else
		{

			_state = State.AIRBORNE;
		}

		float accelerationRate;
		// Continuing in current direction
		if ((movementInput.X > 0) == (Velocity.X > 0))
		{
			accelerationRate = (_walkSpeed - Mathf.Abs(Velocity.X)) / _walkSpeed;
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
				if (GetSprintInput())
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
					_coyoteTimer = 0f;
				}

				vel.X = (float)Mathf.Lerp(vel.X, 0, 0.075);
				vel.X += movementInput.X * accelerationRate * _walkSpeed * _accelerationStrength;

				if (vel.Y < 0)
				{
					vel.Y += _gravityDefault * fDelta;
				}
				else
				{
					vel.Y += _gravityDefault * _gravityFallMultiplier * fDelta;
				}

				break;
		}

		Velocity = vel;

		MoveAndSlide();
	}

	private Godot.Vector2 GetMovementInput()
	{
		Godot.Vector2 movement = new();

		movement.X = Input.GetAxis("ui_left", "ui_right");
		movement.Y = Input.GetAxis("ui_down", "ui_select");
		return movement;
	}

	private bool GetJumpInput()
	{
		return Input.IsActionJustPressed("ui_select");
	}

	private bool GetSprintInput()
	{
		return Input.IsActionPressed("ui_sprint");
	}

	private bool IsSprinting()
	{

		if (IsOnFloor() && GetSprintInput())
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private bool IsWalking()
	{
		if (IsOnFloor() && !IsSprinting())
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private bool IsIdle()
	{
		Godot.Vector2 movementInput = GetMovementInput();
		if (movementInput.X == 0 && IsOnFloor())
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private bool IsJumping()
	{
		if (!IsOnFloor())
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private void ChangePlayerOrientation()
	{
		Godot.Vector2 movementInput = GetMovementInput();
		if (movementInput.X > 0)
		{
			_playerSprite.FlipH = true;
		}
		else if (movementInput.X < 0)
		{
			_playerSprite.FlipH = false;
		}
	}
}
namespace Gameworld;

using Godot;
using Godot.NativeInterop;
using System;
using System.ComponentModel.DataAnnotations.Schema;


public partial class Card : Area2D
{
	[Export] private float _maxOffset = 75f;

	private Hand _handOwner;

	private float _offset;
	public float Offset => _offset;

	private Tween _tween;

	private bool _isMoused;

	public float AngleOffset;
	public Tween AngleTween;
	private int _currentAngleGoal;

	private float _rotationFactor = 0;
	public Tween _rotationFactorTween;
	
	
	public void SetMouseOverStatus(bool moused)
	{		
		if (moused == _isMoused)
			return;

		_isMoused = moused;

		if (moused)
		{
			_tween = CreateTween();
			_tween.TweenProperty(this, "_offset", _maxOffset, 0.25);
			_rotationFactorTween = CreateTween();
			_rotationFactorTween.TweenProperty(this, "_rotationFactor", 1.0f, 0.25f);
		}
		else
		{
			_tween = CreateTween();
			_tween.TweenProperty(this, "_offset", 0f, 0.25);
			_rotationFactorTween = CreateTween();
			_rotationFactorTween.TweenProperty(this, "_rotationFactor", 0f, 0.25f);
		}
	}

	public void SetHandOwner(Hand hand)
	{
		_handOwner = hand;
	}

	public void StartAngleTween(int goal, float duration)
	{
		if (_currentAngleGoal == goal)
			return;

		_currentAngleGoal = goal;

		AngleTween = CreateTween();
		AngleTween.TweenProperty(this, "AngleOffset", goal, duration);
	}

	public float GetRotationFactor()
	{
		return _rotationFactor;
	}

	public bool FullyExtended()
	{
		return _rotationFactor >= 0.95f;
	}

}

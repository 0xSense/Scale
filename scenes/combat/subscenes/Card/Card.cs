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
	


    public void OnMouseEntered()
	{
		_handOwner.CardMousedOver(this);
	}

	public void OnMouseExited()
	{		
		_handOwner.CardUnmousedOver(this);
	}

	public void SetMouseOverStatus(bool moused)
	{		
		if (moused == _isMoused)
			return;

		_isMoused = moused;

		if (moused)
		{
			//if (_offset > _offset*0.1)
				//return;

			_tween = CreateTween();
			_tween.TweenProperty(this, "_offset", _maxOffset, 0.25);
		}
		else
		{
			_tween = CreateTween();
			_tween.TweenProperty(this, "_offset", 0f, 0.25);
		}
	}

	public void SetHandOwner(Hand hand)
	{
		_handOwner = hand;
	}

	public void StartAngleTween(int goal)
	{
		if (_currentAngleGoal == goal)
			return;

		_currentAngleGoal = goal;

		AngleTween = CreateTween();
		AngleTween.TweenProperty(this, "AngleOffset", goal, 0.35);
	}

}

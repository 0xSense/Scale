namespace Overworld;

using Godot;
using System;

public partial class MovingPlatform : StaticBody2D
{
	[Export] private float _traverseDistance;
	[Export] private Vector2 _directionAndSpeed; // Make negative for reversing.
	private Vector2 _startingPosition;
	private Vector2 _centerPoint;

    public override void _Ready()
    {
        _startingPosition = Position;
		_centerPoint = (_startingPosition + (_directionAndSpeed.Normalized()*_traverseDistance)*0.5f);
    }

    public override void _Process(double delta)
    {
		if (ShouldReverse())
		{
			_directionAndSpeed = -_directionAndSpeed;
		}

		Position += _directionAndSpeed * (float)delta;
	}

	private bool ShouldReverse()
	{
		bool headingAwayFromCenterX = ((_directionAndSpeed.X > 0) == (Position.X > _centerPoint.X)) && _directionAndSpeed.X != 0;
		bool headingAwayFromCenterY = ((_directionAndSpeed.Y > 0) == (Position.Y > _centerPoint.Y)) && _directionAndSpeed.Y != 0;
		return Position.DistanceTo(_centerPoint) > _traverseDistance/2f && (headingAwayFromCenterX || headingAwayFromCenterY);
	}
}

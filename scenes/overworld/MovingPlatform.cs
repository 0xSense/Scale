using Godot;
using System;

public partial class MovingPlatform : StaticBody2D
{
	[Export] private float _movingSpeed;
	[Export] private bool _X;
	[Export] private bool _Y;
	[Export] private float _movingDistance;
	private Vector2 _startingPosition; 

    public override void _Ready()
    {
        _startingPosition = Position;
    }

    public override void _PhysicsProcess(double delta)
    {
		if (_X)
		{
			if (_startingPosition.X + _movingDistance >= Position.X)
			{
				Position += new Vector2(_movingSpeed, 0) * (float)delta;
			} 
		} 
		else if (_Y)
		{
			if (_startingPosition.Y + _movingDistance >= Position.Y)
			{
				Position += new Vector2(0, _movingSpeed) * (float)delta;
			} 
		}
		
		

		
		
		
	}
}

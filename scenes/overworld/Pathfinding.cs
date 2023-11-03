using Godot;
using System;

public partial class Pathfinding : CharacterBody2D
{
	[Export] private float _speed;
	[Export] private float _acceleration;
	private Vector2 _direction;
	private NavigationAgent2D _navAgent;
	private Path2D _path;
	private bool _playerInRange;

    public override void _Ready()
    {
        _navAgent = GetNode<NavigationAgent2D>("./NavigationAgent2D");
		_path = GetNode<Path2D>("../../Path2D");
    }

    public override void _Process(double delta)
    {
		if (_playerInRange)
		{
			_direction = new Vector2();
			
			_navAgent.TargetPosition = GetGlobalMousePosition();

			_direction = _navAgent.GetNextPathPosition() - GlobalPosition;
			_direction = _direction.Normalized();

			Velocity = Velocity.Lerp(_direction * _speed, _acceleration * (float)delta);
		}
        MoveAndSlide();
	}

	public void _on_area_2d_body_entered(Node2D body) 
	{
		_playerInRange = true;
	}

	public void _on_area_2d_body_exited(Node2D body) 
	{
		_playerInRange = false;
	}
}

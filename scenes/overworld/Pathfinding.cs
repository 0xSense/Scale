using Godot;
using System;
using System.IO;

public partial class Pathfinding : Node2D
{
	[Export] private float _speed;
	private Vector2 _direction;
	private NavigationAgent2D _navAgent;
	private Path2D _path;
	private CharacterBody2D _enemyBody;
	private bool _playerInRange;
	private CharacterBody2D _playerBody;

    public override void _Ready()
    {
        _navAgent = GetNode<NavigationAgent2D>("./Enemy_Body/NavigationAgent2D");
//		_path = GetNode<Path2D>("./Path2D");
		_enemyBody = GetNode<CharacterBody2D>("./Enemy_Body");
		_playerBody = GetNode<CharacterBody2D>("../Player");
    }

    public override void _Process(double delta)
    {
		if (_playerInRange)
		{
			_direction = new Vector2();

			_navAgent.TargetPosition = _playerBody.GlobalPosition;
			_direction = _navAgent.GetNextPathPosition() - _enemyBody.GlobalPosition;
			_direction = _direction.Normalized();

			_enemyBody.Velocity = _direction * _speed;

			_enemyBody.MoveAndSlide();
		}
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

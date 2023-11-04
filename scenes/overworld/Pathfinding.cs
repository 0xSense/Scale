using Godot;
using System;
using System.IO;
using System.Linq;

public partial class Pathfinding : Node2D
{
	[Export] private float _speed;
	[Export] private float _idleSpeed;
	private Vector2 _direction;
	private NavigationAgent2D _navAgent;
	private PathFollow2D _path;
	private CharacterBody2D _enemyBody;
	private bool _playerInRange;
	private CharacterBody2D _playerBody;
	private Vector2 _startingPos;
	private bool _reachedStartingPos;

    public override void _Ready()
    {
        _navAgent = GetNode<NavigationAgent2D>("./Path2D/PathFollow2D/Enemy_Body/NavigationAgent2D");
		_path = GetNode<PathFollow2D>("./Path2D/PathFollow2D");
		_enemyBody = GetNode<CharacterBody2D>("./Path2D/PathFollow2D/Enemy_Body");
		_playerBody = GetNode<CharacterBody2D>("../Player");
		_startingPos = _enemyBody.GlobalPosition;
    }

    public override void _Process(double delta)
    {
		if (_playerInRange)
		{
			
			_navAgent.TargetPosition = _playerBody.GlobalPosition;
			_direction = _navAgent.GetNextPathPosition() - _enemyBody.GlobalPosition;
			_direction = _direction.Normalized();

			_enemyBody.Velocity = _direction * _speed;

			_enemyBody.MoveAndSlide();

			_reachedStartingPos = false;
		}
		else if ((!_playerInRange) && _reachedStartingPos)
		{
			_path.Progress += _idleSpeed * (float)delta;
		}
		else
		{
			_navAgent.TargetPosition = _startingPos;
			_direction = _navAgent.GetNextPathPosition() - _enemyBody.GlobalPosition;
			_direction = _direction.Normalized();

			_enemyBody.Velocity = _direction * _speed;

			_enemyBody.MoveAndSlide();

			_path.Progress = 0;

			if (_enemyBody.GlobalPosition == _startingPos)
			{
				_reachedStartingPos = true;
			}
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

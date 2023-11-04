using Godot;
using System;

public partial class Pathfinding : Node2D
{
	[Export] private float _speed;
	[Export] private float _acceleration;
	private Vector2 _direction;
	private NavigationAgent2D _navAgent;
	private Path2D _path;
	private CharacterBody2D _enemyBody;
	private bool _playerInRange;

    public override void _Ready()
    {
        _navAgent = GetNode<NavigationAgent2D>("./Enemy_Body/NavigationAgent2D");
		_path = GetNode<Path2D>("./Path2D");
		_enemyBody = GetNode<CharacterBody2D>("./Enemy_Body");
    }

    public override void _Process(double delta)
    {
		if (_playerInRange)
		{
			_direction = new Vector2();
			
			_navAgent.TargetPosition = GetGlobalMousePosition();

			_direction = _navAgent.GetNextPathPosition() - GlobalPosition;
			_direction = _direction.Normalized();

			_enemyBody.Velocity = _enemyBody.Velocity.Lerp(_direction * _speed, _acceleration * (float)delta);
		}
        _enemyBody.MoveAndSlide();
	}

	public void _on_area_2d_body_entered(Node2D body) 
	{
		_playerInRange = true;
	}

	public void _on_area_2d_body_exited(Node2D body) 
	{
		_playerInRange = false;
		GD.Print(body);
		GD.Print("Left Zone");
	}
}

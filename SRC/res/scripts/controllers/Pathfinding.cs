using Godot;

public enum PathState
{
	FOLLOWINGPLAYER,
	IDLE
}

public partial class Pathfinding : Node2D
{
	[Export] private float _speed;
	[Export] private float _idleSpeed;
	[Export] private bool _isWalking;
	[Export] private CharacterBody2D _playerBody;
	private Vector2 _direction;
	private NavigationAgent2D _navAgent;
	private PathFollow2D _pathFollow;
	private CharacterBody2D _enemyBody;
	private float _gravityDefault;
	private PathState _state;

	public override void _Ready()
	{
		_navAgent = GetNode<NavigationAgent2D>("./Body/NavigationAgent2D");
		_enemyBody = GetNode<CharacterBody2D>("./Body");
		_pathFollow = GetNode<PathFollow2D>("./Path2D/PathFollow2D");
		_gravityDefault = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

		_state = PathState.IDLE;
	}

	public override void _Process(double delta)
	{
		Vector2 vel = _enemyBody.Velocity;

		switch (_state)
		{
			case PathState.IDLE:
				_pathFollow.Progress += _idleSpeed * (float)delta;
				if (_isWalking)
				{
					vel.X = FollowPath(_pathFollow.GlobalPosition).X * _idleSpeed;
					vel.Y += _gravityDefault * (float)delta;
				}
				else
				{
					vel = FollowPath(_pathFollow.GlobalPosition) * _idleSpeed;
				}
				break;
			case PathState.FOLLOWINGPLAYER:

				if (_isWalking)
				{
					vel.X = FollowPath(_playerBody.GlobalPosition).X * _speed;
					vel.Y += _gravityDefault * (float)delta;
				}
				else
				{
					vel = FollowPath(_playerBody.GlobalPosition) * _speed;
				}
				break;
		}

		_enemyBody.Velocity = vel;
		_enemyBody.MoveAndSlide();
	}

	public void _on_area_2d_body_entered(Node2D body)
	{
		if (body == _playerBody)
		{
			_state = PathState.FOLLOWINGPLAYER;
		}
	}

	public void _on_area_2d_body_exited(Node2D body)
	{
		if (body == _playerBody)
		{
			_state = PathState.IDLE;
		}
	}

	public Vector2 FollowPath(Vector2 target)
	{
		_navAgent.TargetPosition = target;
		_direction = _navAgent.GetNextPathPosition() - _enemyBody.GlobalPosition;
		_direction = _direction.Normalized();

		return _direction;
	}

}

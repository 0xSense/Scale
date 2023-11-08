using Godot;

// Start Find -> Find ends -> Place Indicator -> Attack Start -> Attack Ends -> Hold -> Repeat

public partial class HandEnemy : Node
{
	[Export] private CharacterBody2D _playerBody;
	[Export] private Marker2D _groundPosition;
	[Export] private Timer _attackTimer;
	[Export] private Timer _findTimer;
	[Export] private Timer _holdPositionTimer;
	private Sprite2D _indicator;
	private CharacterBody2D _enemyBody;
	private bool _playerInRange;
	
	public override void _Ready()
	{
		_indicator = GetNode<Sprite2D>("./Indicator");
		_enemyBody = GetNode<CharacterBody2D>("./Body");
		_enemyBody.Visible = false;
		_indicator.Visible = false;
		_enemyBody.GetNode<CollisionShape2D>("./CollisionShape2D").SetDeferred("disabled", true);
	}

	public void OnAreaBodyEntered(Node2D body)
	{
		if (body == _playerBody)
		{
			GD.Print("Area Enterd");
			_findTimer.Start();
			_holdPositionTimer.Stop();
			_attackTimer.Stop();
		}
	}

	public void OnAreaBodyExited(Node2D body)
	{
		if (body == _playerBody)
		{
			_findTimer.Stop();
			_holdPositionTimer.Stop();
			_attackTimer.Stop();
		}
	}

	public void OnAttackTimerTimeout()
	{
		_indicator.Visible = false;
		_enemyBody.GlobalPosition = new Vector2(_indicator.GlobalPosition.X, _groundPosition.GlobalPosition.Y - 16);
		_enemyBody.Visible = true;
		_enemyBody.GetNode<CollisionShape2D>("./CollisionShape2D").SetDeferred("disabled", false);
		_attackTimer.Stop();
		_holdPositionTimer.Start();
	}

	public void OnFindTimerTimeout()
	{
		_enemyBody.Visible = false;
		_enemyBody.GetNode<CollisionShape2D>("./CollisionShape2D").SetDeferred("disabled", true);
		Vector2 pos = new Vector2(_playerBody.GlobalPosition.X, _groundPosition.GlobalPosition.Y);
		pos.Y -= 1;
		_indicator.GlobalPosition = pos;
		_indicator.Visible = true;
		_findTimer.Stop();
		_attackTimer.Start();
	}

	public void OnHoldPositionTimerTimeout()
	{
		_holdPositionTimer.Stop();
		_findTimer.Start();
	}
}

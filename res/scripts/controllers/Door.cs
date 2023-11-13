using Godot;
using System;

public partial class Door : Node2D
{
	[Export] private Marker2D _targetLocation;
	[Export] private CharacterBody2D _player;
	private bool _playerInsideRange;

	public override void _Process(double delta)
	{
		if (_playerInsideRange)
		{
			if (Input.IsActionJustReleased("ui_interact"))
			{
				_player.Position = _targetLocation.Position;
			}
		}
	}

	public void OnAreaEntered(Node2D body)
	{
		if (body == _player)
		{
			_playerInsideRange = true;
		}
	}

	public void OnAreaExited(Node2D body)
	{
		if (body == _player)
		{
			_playerInsideRange = false;
		}
	}
}

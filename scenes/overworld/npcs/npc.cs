using Godot;
using System;

public partial class npc : Node
{
	[Export] private CharacterBody2D _playerBody;
	private bool _playerInRange;

	public override void _Ready()
	{

	}

	public override void _Process(double delta)
	{
		if (_playerInRange)
		{
			if (Input.IsActionJustPressed("ui_interact"))
			{
				
			}
		}
	}

	public void OnAreaBodyEntered(Node2D body)
	{
		if (body == _playerBody)
		{
			_playerInRange = true;
		}
	}

	public void OnAreaBodyExited(Node2D body)
	{
		if (body == _playerBody)
		{
			_playerInRange = false;
		}
	}
}

using Godot;
using System;
using System.IO;

public partial class npc : Node
{
	[Export] private CharacterBody2D _playerBody;
	[Export] private string _dialogueFile;
	private bool _playerInRange;
	private NinePatchRect _dialogue;
	//private FileStream _json = File.Open("res://Resources/missions.json", FileMode.Open);

	public override void _Ready()
	{
		_dialogue = GetNode<NinePatchRect>("./Dialogue");
		// var text = _json.Read();
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

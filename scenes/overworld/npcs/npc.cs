using Godot;
using Godot.Collections;
using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Text.Json.Nodes;

public partial class npc : Node
{
	[Export] private CharacterBody2D _playerBody;
	[Export] private string _dialogueFilePath;
	[Export] private RichTextLabel _nameField;
	[Export] private RichTextLabel _textField;
	private bool _playerInRange;
	private NinePatchRect _dialogue;
	private string _json; 
	private Godot.Collections.Dictionary _dialogueText = new Godot.Collections.Dictionary();

	public override void _Ready()
	{
		_dialogue = GetNode<NinePatchRect>("./Dialogue");
		_json = Godot.FileAccess.Open(_dialogueFilePath, Godot.FileAccess.ModeFlags.Read).GetAsText();
		var jsonFile = Godot.(_json);
		Dictionary<object, object> Parsed = jsonFile as Dictionary;
		
	}

	public override void _Process(double delta)
	{
		if (_playerInRange)
		{
			if (Input.IsActionJustPressed("ui_interact"))
			{
				GD.Print(_dialogueText["d"]);
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

using Godot;
using System;
using System.Collections.Generic;

public partial class PokerGuyFight : Area2D
{
	// private bool _PlayerHasEntered;
	// [Export] private CharacterBody2D _player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	// public void OnAreaEntered(Area2D body)
	// {
	// 	MasterScene masterScene = ((MasterScene)GetTree().Root.GetChild(0));
	// 	masterScene.SetPlayerHP(10);
	// 	masterScene.SetEnemyIDs(new List<int> { 1, 1, 1, 1 });
	// 	masterScene.ActivatePreviousScene();
	// }

	// public void OnAreaExited(Node2D body)
	// {
	// 	if (body == _player)
	// 	{
	// 		_PlayerHasEntered = false;
	// 	}
	// }
}

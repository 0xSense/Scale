using Godot;
using System;



public partial class TutorialBoss : CharacterBody2D
{
	
	public AnimatedSprite2D _tutorialBossSprite;

		// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_tutorialBossSprite = GetNode<AnimatedSprite2D>("TutorialBossAnimation");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_tutorialBossSprite.Play("Idle");
	}
}

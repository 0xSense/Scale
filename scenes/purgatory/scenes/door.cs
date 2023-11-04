using Godot;
using System;
using System.IO;

public partial class door : Area2D
{
	private bool _doorEntered;
	[Export] private string _sceneTransition;

	public void _on_body_entered(Node2D body)
	{
		GD.Print ("Hello cutie");
		_doorEntered = true;

	}
	public void _on_body_exited(Node2D body)
	{
		GD.Print("Goodbye cutie :(");
		_doorEntered = false;

	}




	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_doorEntered)
		{
			if (Input.IsActionJustReleased("ui_interact"))
			{
				GetTree().ChangeSceneToFile(_sceneTransition);

			}


		}

	}
}

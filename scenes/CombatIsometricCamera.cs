using Godot;
using System;

public partial class CombatIsometricCamera : Camera2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Print the size of the viewport
		GD.Print("Viewport Resolution is: ", GetViewport().GetVisibleRect().Size);
	}

	public override void _Input(InputEvent @event)
	{
		// if (@event is InputEventMouseButton eventMouseButton)
		// {
		// 	GD.Print("Mouse Click/Unclick at:", eventMouseButton.Position);
		// }
		// else
		// {
		// 	GD.Print("Mouse Motion at: ", InputEventMouseMotion.Position);
		// }

	}
}

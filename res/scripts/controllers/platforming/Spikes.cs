namespace Overworld;

using Godot;
using System;

public partial class Spikes : Area2D
{
	public void _on_body_entered(Node2D body)
	{
		GD.Print("1 DMG!!!");
	}
}

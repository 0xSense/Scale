using Godot;
using System;

public partial class ShopKeeperDoor : Node2D
{
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GD.Print("Shoper keeper door" + GetParent());
	}

	public static void OnPlayerChildEnteredTree()
	{
		GD.Print("Hello from this door");
	}
}

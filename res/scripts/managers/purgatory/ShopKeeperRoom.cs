using Godot;
using System;
using System.Linq;

public partial class ShopKeeperRoom : Node2D
{
	private Marker2D _shopKeepEntrancePoint;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print(GetChildren());
		_shopKeepEntrancePoint = GetNode<Marker2D>("./roomspawnpoint");
		GD.Print(_shopKeepEntrancePoint.Position);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

using Godot;
using System;

public partial class Purgatory : Node2D
{

    // Children I want to use
    Node2D _shopKeeperDoor;
    //Signal name creation

    public override void _Ready()
    {
        // GD.Print("Purgatory Scene" + GetChildren());
        // GD.Print(GetNode<Node>("Rooms").GetChildren());
        GD.Print(GetNode<Node>("Doors").GetNode<Node2D>("ShopKeeperDoor"));
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

}
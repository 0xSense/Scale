namespace Combat;

using Data;
using Godot;

public partial class Player : Node2D
{
    [Export] private float HealthValue;
    [Export] private bool Armour;
    [Export] private float ActionPoints;

    public override void _Ready()
    {
        // public static MasterDeck PlayersLibrary{

        // };
    }

    public override void _Process(double delta)
    { }
}
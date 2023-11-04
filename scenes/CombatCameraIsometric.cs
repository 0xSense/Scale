using Godot;
using System;

public partial class CombatCameraIsometric : Camera2D
{
    public Godot.Resource MousePointer = ResourceLoader.Load("res://scenes/combat/asset/sharp.png");

    public override void _Ready()
    {
        Input.SetCustomMouseCursor(MousePointer);
    }

}

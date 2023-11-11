using Godot;
using System;

public partial class CombatCameraIsometric : Camera2D
{
    public Godot.Resource MousePointer = ResourceLoader.Load("res://scenes/combat/asset/sharp.png");

    public override void _Ready()
    {
        Input.SetCustomMouseCursor(MousePointer);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton)
        {
            GD.Print("Mouse Click/Unclick at: ", eventMouseButton.Position);
        }
        else if (@event is InputEventMouseMotion eventMouseMotion)
        {
            GD.Print("Viewport Resolution is: ", GetViewport().GetVisibleRect().Size);
        }
        GetViewport().GetMousePosition();
    }

    public override void _EnterTree()
    {
        base._EnterTree();
    }
}

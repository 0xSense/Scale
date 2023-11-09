using Godot;
using System;

public partial class TestLevel : Node2D
{
    [Export] private string _combatScene;
    bool _listeningToInput;

    public override void _Process(double delta)
    {
        if (Input.IsPhysicalKeyPressed(Key.Delete))
        {
            if (_listeningToInput)
                ((MasterScene)GetTree().Root.GetChild(0)).ActivateScene(_combatScene, true);

            _listeningToInput = false;
        }
        else
            _listeningToInput = true;
    }
}

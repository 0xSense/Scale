using Combat;
using Data;
using Godot;
using System;
using System.Collections.Generic;

public partial class TestLevel : Node2D
{
    [Export] private string _combatScene;
    bool _listeningToInput;

    public override void _Process(double delta)
    {
        if (Input.IsPhysicalKeyPressed(Key.Delete))
        {
            MasterScene master = (MasterScene)GetTree().Root.GetChild(0);
            master.SetPlayerHP(10);
            master.SetEnemyIDs(new List<int>{1, 1, 1});
            if (_listeningToInput)
                ((MasterScene)GetTree().Root.GetChild(0)).ActivateScene(_combatScene, true);

            _listeningToInput = false;
        }
        else
            _listeningToInput = true;
    }
}

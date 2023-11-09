/*
 @author Alexander Venezia (Blunderguy)
*/

namespace Combat;

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Systems.Combat;


public partial class CombatMain : Node
{
    [ExportGroup("Right click .tscn -> get UID -> paste here. Blame Godot for having to use freaking strings because they still haven't fixed a bug reported over a year ago: https:[slashslash]github.com[slash]godotengine[slash]godot[slash]issues[slash]62916")]
    [Export] private Godot.Collections.Array<string> _enemyTypePaths;
    private Dictionary<int, PackedScene> _enemyTypesByUID;
    private void PopulateEnemyArray(ref ICombatant[] enemies)
    {

        _enemyTypesByUID = new();
        
        // This is stupid, but that's Godot's fault.
        for (int i = 0; i < _enemyTypePaths.Count; i++)
        {
            PackedScene converted = GD.Load<PackedScene>(_enemyTypePaths[i]);
            if (converted == null)
                continue;

            Enemy tempInstance = (Enemy)converted.Instantiate();

            if (_enemyTypesByUID.ContainsKey(tempInstance.UID))
                throw new Exception("Two enemies conflict with shared UID " + tempInstance.UID);

            _enemyTypesByUID.Add(tempInstance.UID, converted);
            tempInstance.QueueFree();
        }
        Node enemiesParentNode = GetNode("Enemies");

        // TODO: Finish this once you have the scene transition implemented
        /* 
        int[] unloadedEnemies = (Whatever the master root's class is) GetTree().Root.GetChild(0).GetEnemyIndices();

        enemies = new ICombatant[unloadedEnemies.Length];

        int count = 0;
        foreach (int e in unloadedEnemies)
        {
            Enemy newEnemy = (Enemy)_enemyTypesByUID[e].Instantiate();
            RemoveChild(newEnemy);
            enemiesParentNode.AddChild(newEnemy);
            enemies[count] = newEnemy;
            count++;
        }
        */

        // For testing purposes:

        enemies = new ICombatant[1];
        
        Enemy newEnemy = (Enemy)GD.Load<PackedScene>(_enemyTypePaths[0]).Instantiate();
        enemiesParentNode.AddChild(newEnemy);
        enemies[0] = newEnemy;

    }

    /* TEST - TODO REMOVE*/
    public override void _Process(double delta)
    {
         if (Input.IsPhysicalKeyPressed(Key.Space))
        {
            ((MasterScene)GetTree().Root.GetChild(0)).ActivatePreviousScene();
        }
    }

    public override void _Ready()
    {
        ICombatant[] enemies = null;
        PopulateEnemyArray(ref enemies);

        CombatManager.GetInstance().NewFight((ICombatant)GetNode("Player"), enemies);
    }

    public void SetPlayerHP(int hp)
    {
        ((Player)GetNode("Player")).SetHP(hp);
    }

    public int GetPlayerHP()
    {
        return ((Player)GetNode("Player")).CurrentHealth;
    }

}

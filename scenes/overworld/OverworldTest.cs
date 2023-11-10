using Godot;
using System;

public partial class OverworldTest : Node
{
    public void SetPlayerHP(int hp)
    {        
        ((Label)GetNode("Label")).Text = "HP: " + hp;
    }

}

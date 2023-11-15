using Combat;
using Godot;
using System;

public partial class Health : Sprite2D
{
    private Combatant _parent;
    private Label _label;

    public override void _Ready()
    {
        _parent = (Combatant)GetParent().GetParent();
        _label = (Label)GetChild(0);
        Visible = false;
    }
    public override void _Process(double delta)
    {
        Visible = true;
        int armor = _parent.GetHealth();
        if (armor <= 0)
            Visible = false;
        
        _label.Text = armor.ToString();
        
    }
}

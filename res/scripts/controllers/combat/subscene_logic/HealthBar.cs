using Combat;
using Godot;
using System;

public partial class HealthBar : Sprite2D
{
    private Combatant _parent;

    public override void _Ready()
    {
        _parent = (Combatant)GetParent();
    }
    public override void _Process(double delta)
    {
        // GD.Print(_parent.GetHealth() + "; " + _parent.MaxHealth + "; " + (float)_parent.GetHealth() / (float)_parent.MaxHealth);
        (Material as ShaderMaterial).SetShaderParameter("health_factor", (float)_parent.GetHealth() / (float)_parent.MaxHealth);
    }
}

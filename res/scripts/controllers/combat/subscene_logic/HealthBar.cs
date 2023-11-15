using Combat;
using Godot;
using System;

public partial class HealthBar : Sprite2D
{
    private Combatant _parent;

    public override void _Ready()
    {
        _parent = (Combatant)GetParent().GetParent();
    }
    public override void _Process(double delta)
    {
        (Material as ShaderMaterial).SetShaderParameter("health_factor", (float)_parent.GetHealth() / (float)_parent.MaxHealth);
    }
}

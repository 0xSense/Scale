using Combat;
using Data;
using Godot;
using System;
using System.Diagnostics;

public partial class HealthBar : Sprite2D
{
    private Combatant _parent;

    private RichTextLabel buffs;
    private RichTextLabel debuffs;
    private float _currentHealthTrack;

    public override void _Ready()
    {
        _parent = (Combatant)GetParent().GetParent();
        buffs = (RichTextLabel)GetNode("../Buffs/RichTextLabel");
        debuffs = (RichTextLabel)GetNode("../Debuffs/RichTextLabel");
        _currentHealthTrack = -1;
    }
    public override void _Process(double delta)
    {
        if (_currentHealthTrack < 0) _currentHealthTrack = (float)_parent.GetHealth() / (float)_parent.MaxHealth;
        float currentHealth = (float)_parent.GetHealth() / (float)_parent.MaxHealth;

        if (Mathf.Abs(currentHealth-_currentHealthTrack) < 0.01f)
        {
            _currentHealthTrack = currentHealth;
        }
        else if (_currentHealthTrack < currentHealth)
            _currentHealthTrack += (float)delta * 0.5f;
        else if (_currentHealthTrack > currentHealth)
            _currentHealthTrack -= (float)delta * 0.5f;

        float drain = Math.Max(_currentHealthTrack-currentHealth, 0);
        //drain = 0f;

        ShaderMaterial mat = (ShaderMaterial)Material;
        mat.SetShaderParameter("health_factor", currentHealth);
        mat.SetShaderParameter("draining_factor", drain);

        string text = "[color=#22BB22]";

        foreach (BuffType b in _parent.Buffs.Keys)
        {
            if (_parent.Buffs[b] > 0)
                text += b + " : " + _parent.Buffs[b] + "\n";
        }
        text += "[/color]";
        buffs.Text = text;

        text = "[right][color=#BB2222]";

        foreach (DebuffType d in _parent.Debuffs.Keys)
        {
            if (_parent.Debuffs[d] > 0)
                text += d + " : " + _parent.Debuffs[d] + "\n";
        }
        text += "[/color]";
        debuffs.Text = text;

    }
}

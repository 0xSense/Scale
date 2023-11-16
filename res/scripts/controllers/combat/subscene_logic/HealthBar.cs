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

    public override void _Ready()
    {
        _parent = (Combatant)GetParent().GetParent();
        buffs = (RichTextLabel)GetNode("../Buffs/RichTextLabel");
        debuffs = (RichTextLabel)GetNode("../Debuffs/RichTextLabel");

        GD.Print(buffs);

    }
    public override void _Process(double delta)
    {
        (Material as ShaderMaterial).SetShaderParameter("health_factor", (float)_parent.GetHealth() / (float)_parent.MaxHealth);

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

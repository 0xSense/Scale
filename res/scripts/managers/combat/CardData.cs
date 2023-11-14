/*
 @author Alexander Venezia (Blunderguy)
*/

namespace Data;

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public enum CardRarity
{
    COMMON,
    UNCOMMON,
    RARE,
    EPIC,
    LEGENDARY
}

public enum CardType
{
    MOVEMENT,
    CLASS,
    ACTION,
    PACKAGE
}

public enum DamageType
{
    DIVINE,
    DEMONIC,
    FIRE,
    POISON,
    LIGHTNING,
    SHARP,
    BLUNT,
    PIERCING,
    HEAL
}

public enum TargetType
{
    SELF,
    SINGLE,
    MULTI_TWO,
    MULTI_THREE,
    MULTI_FOUR,
    ALL // All enemies and yourself
}

public enum DrawEffect
{
    DRAW,
    RETURN,
    DISCARD,
    GRAB // Implementation as stretch goal
}

public enum BuffType
{
    RESISTANCE,
    ARMOR,
    CRIT_DMG_INCREASE,
    CRIT_CHANCE_INCREASE,
    PURIFY
}

public enum DebuffType
{
    POISONED,
    CRIPPLED,
    TRAUMATIZED,
    SHOCKED,
    EXPOSED,
    WITHER,
}

public struct DamageDice
{
    public int d4;
    public int d6;
    public int d8;
    public int d10;
    public int flat; // Flat addition, as in "2d6 + *5*"

    public override string ToString()
    {
        string final = "";

        if (d4 > 0)
            final += d4 + "d4";
        if (d6 > 0)
            final += d6 + "d6";
        if (d8 > 0)
            final += d8 + "d8";
        if (d10 > 0)
            final += d10 + "d10";

        if (flat != 0)
        {
            if (flat > 0)
                final += " + " + flat;
            else
                final += " - " + Math.Abs(flat);
        }

        return final;
    }
}

public struct Resistance
{
    public DamageType Type;
    public int Duration;

    public Resistance(DamageType type, int duration)
    {
        Type = type;
        Duration = duration;
    }
}

public struct Buff
{
    public BuffType Type;
    public int Value; // If armor, this represents amount of armor gained; otherwise, duration
    public DamageType ResistanceType;
}

public struct Debuff
{
    public DebuffType Type;
    public int Duration;
}


public partial class CardData : Resource
{
    [ExportGroup("Appearance")]
    [Export] private string _name;
    [Export] private string _description;
    [Export] private Texture2D _artwork;
    public Texture2D Artwork => _artwork;

    [ExportGroup("Attributes")]
    // Types and rarity
    [Export] private CardType _type;
    [Export] private CardRarity _rarity;
    [Export] private int _actionPointCost;
    [Export] private int _movementPointCost;

    public string Name => _name;
    public string Description => _description;

    public CardRarity Rarity => _rarity;
    public CardType Type => _type;
    public int ActionPointCost => _actionPointCost;
    public int MovementPointCost => _movementPointCost;

    [Export] private TargetType _target;
    public TargetType Target => _target;

    [ExportGroup("Damage")]
    // Damage and damage types
    [Export] private Godot.Collections.Array<DamageType> _damageTypes = new();
    [Export] private Godot.Collections.Array<String> _damageDice = new();
    public Dictionary<DamageType, DamageDice> Damage;

    [ExportGroup("Draw")]
    // Draw effects (draw/remove cards to deck; always affects player regardless of TargetType)
    [Export] private Godot.Collections.Array<DrawEffect> _drawEffects = new();
    [Export] private Godot.Collections.Array<int> _drawQuantities = new();
    public Dictionary<DrawEffect, int> DrawEffects;

    [ExportGroup("Buff")]
    // Buff type/duration
    [Export] private Godot.Collections.Array<BuffType> _buffs = new();
    [Export] private Godot.Collections.Array<int> _buffDuration = new(); // Turn based, not seconds
    [ExportGroup("Leave null if buff is not type resistance")]
    [Export] private Godot.Collections.Array<DamageType> _resistanceType = new();
    //public Dictionary<BuffType, int> Buffs;
    public List<Buff> Buffs;

    [ExportGroup("Debuff")]
    // Debuff type/duration
    [Export] private Godot.Collections.Array<DebuffType> _debuffs = new();
    [Export] private Godot.Collections.Array<int> _debuffDuration = new(); // Turn based, not seconds
    // public Dictionary<DebuffType, int> Debuffs;
    public List<Debuff> Debuffs;


    private int _uid;
    public int UID => _uid;
    private static int _usedIds = 0;

    public CardData()
    {
        _uid = _usedIds++;
    }

    public void Activate()
    {
          Damage = new();
        DrawEffects = new();
        Buffs = new();
        Debuffs = new();

        int i = 0;
        DamageDice dice;
        int dieQuantity;
        int dieDenomination;
        int flatModifier;


        foreach (DamageType t in _damageTypes)
        {
            dice = new();

            string[] parts = _damageDice[i].Split("d");

            if (parts.Length < 2)
            {
                dice.flat = int.Parse(parts[0].Trim());
                Damage.Add(t, dice);
                continue;
            }

            dieQuantity = int.Parse(parts[0]);

            if (parts[1].Contains("+"))
            {
                parts = parts[1].Split("+");
                dieDenomination = int.Parse(parts[0].Trim());
                flatModifier = int.Parse(parts[1].Trim());
            }
            else if (parts[1].Contains("-"))
            {
                parts = parts[1].Split("-");
                dieDenomination = int.Parse(parts[0].Trim());
                flatModifier = int.Parse(parts[1].Trim());
                flatModifier = -flatModifier;
            }
            else
            {
                dieDenomination = int.Parse(parts[1].Trim());
                flatModifier = 0;
            }

            switch (dieDenomination)
            {
                case 4:
                    dice.d4 = dieQuantity;
                    break;
                case 6:
                    dice.d6 = dieQuantity;
                    break;
                case 8:
                    dice.d8 = dieQuantity;
                    break;
                case 10:
                    dice.d10 = dieQuantity;
                    break;
            }

            dice.flat = flatModifier;
            Damage.Add(t, dice);

            i++;
        }

        // _damageTypes.Clear();
        // _damageDice.Clear();

        i = 0;
        foreach (DrawEffect e in _drawEffects)
        {
            DrawEffects.Add(e, _drawQuantities[i++]);
        }

        // _drawEffects.Clear();
        // _drawQuantities.Clear();

        i = 0;
        foreach (BuffType b in _buffs)
        {
            // Buffs.Add(b, _buffDuration[i++]);
            Buff nB = new();
            nB.Type = b;
            nB.Value = _buffDuration[i];
            if (b == BuffType.RESISTANCE)
                nB.ResistanceType = _resistanceType[i];

            Buffs.Add(nB);
            i++;
        }

        // _buffs.Clear();
        // _buffDuration.Clear();

        i = 0;
        foreach (DebuffType d in _debuffs)
        {
            Debuff dB = new();
            dB.Type = d;
            dB.Duration = _debuffDuration[i];
            Debuffs.Add(dB);
            i++;
        }

        // GD.Print(_name + "; " + _damageDice.Count);

        // _debuffs.Clear();
        // _debuffDuration.Clear();
    }

    // Operator overrides for easy comparisons of card data. Don't worry about the implementation here unless you really think it's bugged.
    public override bool Equals(object obj)
    {
        if (obj is not CardData)
            return false;
        return _uid == ((CardData)obj).UID;
    }

    public static bool operator ==(CardData c1, CardData c2)
    {
        if ((object)c1 == null)
            return (object)c2 == null;
        return c1.Equals(c2);
    }

    public static bool operator !=(CardData c1, CardData c2)
    {
        return !(c1 == c2);
    }

    public override int GetHashCode()
    {
        return _uid.GetHashCode();
    }

    public override string ToString()
    {
        return "Card " + _name;
    }

}



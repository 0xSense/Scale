namespace Data;

using Godot;
using System;
using System.Collections.Generic;


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
    PIERCING
}


public partial class CardData : Resource
{    
    [Export] private String _name;
    [Export] private CardType _type;
    [Export] private CardRarity _rarity;
    [Export] private Godot.Collections.Array<DamageType> _damageTypes = new();
    [Export] private Godot.Collections.Array<int> _damageValues = new();


    public CardRarity Rarity => _rarity;
    public CardType Type => _type;
    public String Name => _name;
    public Dictionary<DamageType, int> Damage;

    private int _uid;
    public int UID => _uid;
    private static int _usedIds = 0;

    public CardData() : this(CardRarity.COMMON, CardType.CLASS)
    {
        Damage = new();
        int i = 0;
        foreach (DamageType t in _damageTypes)
        {
            Damage.Add(t, _damageValues[i++]);
        }

        _damageTypes.Clear();
        _damageValues.Clear();
    }

    public CardData(CardRarity rarity, CardType type) : base()
    {
        _uid = _usedIds++;
        //GD.Print("Card: " + _uid);
    }

    // Operator overrides for easy comparisons of card data. Don't worry about the implementation here unless you really think it's bugged.
    public override bool Equals(object obj)
    {
        if (obj is not CardData)
            return false;
        return _uid == ((CardData)obj).UID;
    }

    public static bool operator == (CardData c1, CardData c2)
    {
        if ((object)c1 == null)
            return (object)c2 == null;
        return c1.Equals(c2);
    }

    public static bool operator != (CardData c1, CardData c2)
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



namespace Data;

using Godot;
using System;

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

public partial class CardData : Resource
{    
    [Export] private CardRarity _rarity;
    [Export] private CardType _type;

    [Export] private String _name;

    public CardRarity Rarity => _rarity;
    public CardType Type => _type;
    public String Name => _name;

    private int _uid;
    public int UID => _uid;
    private static int _usedIds = 0;

    public CardData() : this(CardRarity.COMMON, CardType.CLASS)
    {}

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



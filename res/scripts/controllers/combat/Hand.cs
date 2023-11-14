namespace Systems.Combat;

using System.Collections.Generic;
using System.Linq;
using Data;

public class Hand
{
    public LinkedList<CardData> _hand;
    public LinkedList<CardData> CurrentHand => _hand;

    public Hand()
    {

    }

    public CardData Play(int index)
    {
        CardData played = _hand.ElementAt(index);

        return played;
    }

}
using System.Collections.Generic;
using Data;

public class Hand
{
    public LinkedList<CardData> _hand;
    public LinkedList<CardData> CurrentHand => _hand;

    public Hand()
    {

    }

}
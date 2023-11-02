/*
Represents the cards carried by a player, enemy, etc
*/

namespace Systems.Combat;

using System.Collections.Generic;
using Data;

public class Deck
{
    private Dictionary<CardData, int> _cards; // Maps cards to number of instances in this deck
    
    public Deck()
    {
        _cards = new();
    }

    public void AddCard(CardData card)
    {
        
    }
}
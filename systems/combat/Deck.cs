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
        if (_cards.ContainsKey(card))
            _cards[card]++;
        else
            _cards.Add(card, 1);
    }

    public void RemoveCard(CardData card)
    {
        if (_cards.ContainsKey(card))
        {
            _cards[card]--;
            if (_cards[card] <= 0)
                _cards.Remove(card);
        }
    }

    public int GetCount(CardData card)
    {
        if (!_cards.ContainsKey(card))
            return 0;
        return _cards[card];
    }

}
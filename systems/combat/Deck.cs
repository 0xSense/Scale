/*
Represents the cards carried by a player, enemy, etc
*/

namespace Systems.Combat;

using System.Collections.Generic;
using Data;

public class Deck
{
    private Dictionary<CardData, int> _cards; // Maps cards to number of instances in this deck
    public Dictionary<CardData, int> CardDict => _cards;


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

    /* Returns all cards */
    public List<CardData> GetCards()
    {
        return GetCards((CardData c) => true);
    }


    /*
    Generic filtered card getter. Example to get all action cards with an action point cost greater than 2:

    GetCards(
        (CardData c) => {
            return c.Type == CardType.ACTION && c.ActionPointCost > 2
        }

    )

    Filter(CardData) -> bool; returns true if card is to be included in return, else otherwise.
    */
    public List<CardData> GetCards(System.Func<CardData, bool> filter)
    {
        List<CardData> cards = new();

        foreach (CardData c in _cards.Keys)
        {
            for (int i = 0; i < _cards[c]; i++)
            {
                if (filter(c))
                    cards.Add(c);
            }
        }

        return cards;
    }

}
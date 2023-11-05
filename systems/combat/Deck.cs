/*
Represents the cards carried by a player, enemy, etc
*/

namespace Systems.Combat;

using System;
using System.Collections.Generic;
using Data;

public class Deck
{
    private Dictionary<CardData, int> _cards; // Maps cards to number of instances in this deck
    public Dictionary<CardData, int> CardDict => _cards;

    private LinkedList<CardData> _mainDeck;
    private List<CardData> _discard;


    public Deck()
    {
        _cards = new();
        _mainDeck = new();
        _discard = new();
    }

    public void AddCard(CardData card)
    {
        if (_cards.ContainsKey(card))
            _cards[card]++;
        else
            _cards.Add(card, 1);

        _mainDeck.AddFirst(card);
    }

    public void RemoveCard(CardData card)
    {
        if (_cards.ContainsKey(card))
        {
            _cards[card]--;
            if (_cards[card] <= 0)
                _cards.Remove(card);
        }

        CardData toRemove = null;
        foreach (CardData c in _mainDeck)
        {
            if (c == card)
            {
                toRemove = c;
                break;
            }
        }

        if (toRemove != null)
            _mainDeck.Remove(toRemove);
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

    public CardData[] Draw(int numCards)
    {
        Random rand = CombatManager.GetInstance().RNG;
        CardData[] cards = new CardData[numCards];


        // TODO: Implement. Draw in order from _mainDeck. If it empties, shuffle _discard and swap before continuing to draw.

        // _mainDeck.RemoveLast();


        return cards;
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
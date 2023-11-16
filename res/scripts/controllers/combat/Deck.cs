/*
@author Alexander Venezia (Blunderguy)
Represents the cards carried by a player, enemy, etc
*/

namespace Systems.Combat;

using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Godot;


public class Deck
{
    private Dictionary<CardData, int> _cards; // Maps cards to number of instances in this deck
    public Dictionary<CardData, int> CardDict => _cards;
    private LinkedList<CardData> _mainDeck;
    public LinkedList<CardData> CardList => _mainDeck;
    private List<CardData> _discard;

    public delegate void DiscardChangeWarner();
    public event DiscardChangeWarner OnDiscardChange;

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

    public void RemoveCard(CardData card, bool removeFromMainDeck)
    {
        if (_cards.ContainsKey(card))
        {
            _cards[card]--;
            if (_cards[card] <= 0)
                _cards.Remove(card);
        }

        if (!removeFromMainDeck)
            return;

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

    public void Discard(CardData[] cards)
    {
        foreach (var c in cards)
            Discard(c);
    }

    public void Discard(CardData card)
    {
        _discard.Add(card);
    }

    public int GetCardCount()
    {
        return _mainDeck.Count();
    }

    public int GetDiscardCount()
    {
        return _discard.Count();
    }

    public CardData DrawSpecific(CardData card)
    {
        RemoveCard(card, true);
        return card;
    }

    public CardData[] Draw(int numCards)
    {
        Random rand = CombatManager.GetInstance().RNG;
        CardData[] cards = new CardData[numCards];

        for (int i = 0; i < numCards; i++)
        {
            ShuffleIfNecessary();

            if (_mainDeck.Count <= 0)
            {
                cards[i] = null;
                continue;
            }
            cards[i] = _mainDeck.Last.Value;
            _mainDeck.RemoveLast();
            RemoveCard(cards[i], false);

        }

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

    public void ForceShuffle()
    {
        Shuffle(_discard);
        foreach (CardData c in _discard)
        {
            AddCard(c);
        }

        _discard.Clear();
        OnDiscardChange?.Invoke();
    }

    public void ForceFullReshuffle()
    {
        Discard(Draw(_mainDeck.Count()));
        ShuffleIfNecessary();
    }

    private void ShuffleIfNecessary()
    {
        if (_mainDeck.Count <= 0)
        {
            Shuffle(_discard);
            foreach (CardData c in _discard)
            {
                AddCard(c);
            }

            _discard.Clear();
            OnDiscardChange?.Invoke();
            GD.Print("In shuffle");
            GD.Print(OnDiscardChange == null);
        }
    }

    public static void Shuffle(List<CardData> list)
    {
        int n = list.Count;
        
        while (n > 1)
        {
            n--;
            int k = CombatManager.GetInstance().RNG.Next(n + 1);
            CardData swap = list[k];
            list[k] = list[n];
            list[n] = swap;
        }
    }
}
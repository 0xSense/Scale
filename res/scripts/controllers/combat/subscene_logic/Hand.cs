/*
 @author Alexander Venezia (Blunderguy)
*/

namespace Combat;

using Data;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using Systems.Combat;

public partial class Hand : Marker2D
{
    [Export] private int _openingHandSize = 7;
    [Export] private PackedScene _cardResource;
    [Export] private Vector2 _scale;
    [Export] private float _handArc;
    [Export] private float _radius;
    private List<Card> _cards;

    private int _selectedCardIndex;
    private bool _frozen;

    public override void _Ready()
    {
        _selectedCardIndex = -1;
        _cards = new();
        _frozen = false;

        // FillHandDefault();
    }

    public void DrawOpeningHand(Deck deck)
    {
        CardData[] cards = deck.Draw(_openingHandSize);
        foreach (CardData c in cards)
            AddCards(c, 1);

        // for (int i = 0; i < _openingHandSize; i++)
    }

    private void FillHandDefault()
    {
        AddCards(MasterDeck.CardTypes[0], 1);
        AddCards(MasterDeck.CardTypes[1], 1);
        AddCards(MasterDeck.CardTypes[2], 1);
        AddCards(MasterDeck.CardTypes[3], 1);
        AddCards(MasterDeck.CardTypes[4], 1);
        AddCards(MasterDeck.CardTypes[5], 1);
    }

    public void AddCards(CardData card, int n)
    {
        Card newCard;
        for (int i = 0; i < n; i++)
        {
            newCard = (Card)_cardResource.Instantiate();
            newCard.UpdateData(card);
            newCard.SetHandOwner(this);
            AddChild(newCard);
            _cards.Add(newCard);
        }

        OrderCards();
    }

    public void AddCards(CardData[] cards)
    {
        foreach (CardData c in cards)
        {
            if (c != null)
                AddCards(c, 1);
        }
    }

    public CardData RemoveCard(Card card, bool delete=false, Node2D discard=null)
    {
        RemoveChild(card);
        _cards.Remove(card);
        _selectedCardIndex = -1;
        OrderCards();

        if (discard != null)
        {
            card.AddToDiscard(discard);
        }
        else if (delete)
            card.QueueFree();

        return card.Data;

    }

    public void Freeze()
    {
        _frozen = true;
    }
    public void Unfreeze()
    {
        _frozen = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_frozen)
        {
            UpdateCardProtrusion();
            OrderCards();
            return;
        }

        Vector2 mousePos = GetGlobalMousePosition();
        PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
        PhysicsPointQueryParameters2D query = new();
        query.CollideWithAreas = true;
        query.Position = mousePos;
        query.CollisionMask = 0b0010;
        var hits = spaceState.IntersectPoint(query);

        Card highestZ = null;
        Card nextCard = null;

        foreach (var h in hits)
        {
            nextCard = (Card)(h["collider"]);
            if (highestZ == null || nextCard.ZIndex > highestZ.ZIndex)
                highestZ = nextCard;
        }

        if (highestZ != null)
            _selectedCardIndex = _cards.IndexOf(highestZ);
        else
            _selectedCardIndex = -1;

        UpdateCardProtrusion();
        OrderCards();

    }

    private void OrderCards()
    {
        float arcStep = _handArc / _cards.Count;
        float arcStart = -(_handArc / 2f);

        Vector2 offset = Vector2.Zero;

        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].Scale = _scale;
            float angle = arcStart + arcStep * (i + 0.5f);

            if (_selectedCardIndex != -1 && i < _selectedCardIndex)
            {
                _cards[i].StartAngleTween(-(int)((_cards.Count / 2) * 1.4), 0.25f);

            }
            else if (_selectedCardIndex != -1 && i > _selectedCardIndex)
            {
                _cards[i].StartAngleTween((int)((_cards.Count / 2) * 1.4), 0.25f);
            }
            else
                _cards[i].StartAngleTween(0, 0.25f);

            _cards[i].RotationDegrees = Mathf.Lerp(angle, 0, _cards[i].GetRotationFactor());

            offset.X = Mathf.Cos(Mathf.DegToRad(angle - 90 + _cards[i].AngleOffset)) * (_radius + _cards[i].Offset) * 1.25f;
            offset.Y = Mathf.Sin(Mathf.DegToRad(angle - 90 + _cards[i].AngleOffset)) * (_radius + _cards[i].Offset);

            _cards[i].Position = Vector2.Zero + offset;

            _cards[i].ZIndex = i + 1;

        }

        if (_selectedCardIndex != -1 && _cards[_selectedCardIndex].FullyExtended())
        {
            _cards[_selectedCardIndex].ZIndex = _cards.Count + 2;
        }
    }


    private void UpdateCardProtrusion()
    {
        Card topMoused = null;

        if (_selectedCardIndex >= 0)
            topMoused = _cards[_selectedCardIndex]; //GetTopMoused();

        foreach (Card c in _cards)
        {
            if (c != topMoused)
                c.SetMouseOverStatus(false);
        }

        topMoused?.SetMouseOverStatus(true);
        if (topMoused == null)
        {
            _selectedCardIndex = -1;
            return;
        }
        _selectedCardIndex = _cards.IndexOf(topMoused);
    }

    public Card GetSelectedCard()
    {
        if (_selectedCardIndex == -1) return null;
        return _cards[_selectedCardIndex];
    }

    public int GetCount()
    {
        return _cards.Count();
    }


}

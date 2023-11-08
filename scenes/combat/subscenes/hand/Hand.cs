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


public partial class Hand : Marker2D
{   
    [Export] private PackedScene _cardResource;
    [Export] private Vector2 _scale;
    [Export] private float _handArc;
    [Export] private float _radius;
    private List<Card> _cards;

    private int _selectedCardIndex;

    public override void _Ready()
    {
        _selectedCardIndex = -1;
        _cards = new();

        MasterDeck.OnLoad += FillHandDefault;
    }

    private void FillHandDefault()
    {
        AddCards(MasterDeck.CardTypes[0], 2);
        AddCards(MasterDeck.CardTypes[1], 3);
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

    public override void _PhysicsProcess(double delta)
    {
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
        float arcStep = _handArc/_cards.Count;
        float arcStart = -(_handArc/2f);

        Vector2 offset = Vector2.Zero;

        for (int i = 0; i < _cards.Count; i++)
        {
            _cards[i].Scale = _scale;
            float angle = arcStart + arcStep*(i+0.5f);

            if (_selectedCardIndex != -1 && i < _selectedCardIndex)
            {
                _cards[i].StartAngleTween(-(int)((_cards.Count/2)*1.4), 0.25f);

            }
            else if (_selectedCardIndex != -1 && i > _selectedCardIndex)
            {
                _cards[i].StartAngleTween((int)((_cards.Count/2)*1.4), 0.25f);
            }
            else
                _cards[i].StartAngleTween(0, 0.25f);

            _cards[i].RotationDegrees = Mathf.Lerp(angle, 0, _cards[i].GetRotationFactor());

            offset.X = Mathf.Cos(Mathf.DegToRad(angle-90+_cards[i].AngleOffset)) * (_radius+_cards[i].Offset) * 1.25f;
            offset.Y = Mathf.Sin(Mathf.DegToRad(angle-90+_cards[i].AngleOffset)) * (_radius+_cards[i].Offset);

            _cards[i].Position = Vector2.Zero + offset;

            _cards[i].ZIndex = i+1;  

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

}

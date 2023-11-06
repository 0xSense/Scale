namespace Gameworld;

using Data;
using Godot;
using System;
using System.Collections.Generic;


public partial class Hand : Node
{   
    [Export] private PackedScene _cardResource;
    [Export] private Vector2 _scale;
    [Export] private float _handArc;
    [Export] private float _radius;
    private List<Card> _cards;
    private List<bool> _mousedOver;

    private int _selectedCardIndex;

    public override void _Ready()
    {
        _selectedCardIndex = -1;
        _cards = new();
        _mousedOver = new();

        MasterDeck.OnLoad += FillHandDefault;
    }

    private void FillHandDefault()
    {
        AddCards(MasterDeck.CardTypes[0], 6);
    }

    public void AddCards(CardData card, int n)
    {
        Card newCard;
        for (int i = 0; i < n; i++)
        {
            newCard = (Card)_cardResource.Instantiate();
            newCard.SetHandOwner(this);
            AddChild(newCard);
            _cards.Add(newCard);
            _mousedOver.Add(false);
        }

        OrderCards();
    }

    public override void _Process(double delta)
    {
        OrderCards();

        UpdateCardProtrusion();
        
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
                _cards[i].StartAngleTween(-(int)(_cards.Count*1.75));

            }
            else if (_selectedCardIndex != -1 && i > _selectedCardIndex)
            {
                _cards[i].StartAngleTween((int)(_cards.Count*1.75));
            }
            else
                _cards[i].StartAngleTween(0);

            _cards[i].RotationDegrees = angle;

            offset.X = Mathf.Cos(Mathf.DegToRad(angle-90+_cards[i].AngleOffset)) * (_radius+_cards[i].Offset) * 1.5f;
            offset.Y = Mathf.Sin(Mathf.DegToRad(angle-90+_cards[i].AngleOffset)) * (_radius+_cards[i].Offset);

            _cards[i].Position = Vector2.Zero + offset;

            _cards[i].ZIndex = i+1;  

        }
    }

    private void UpdateCardProtrusion()
    {
        Card topMoused = GetTopMoused();

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

    public void PrintOnMousedDebug()
    {
        string msg = "";
        foreach (bool v in _mousedOver)
        {
            msg += v;
        }
        GD.Print(msg, "\n");
    }    

    public void CardMousedOver(Card c)
    {
        _mousedOver[_cards.IndexOf(c)] = true;
    }

    public void CardUnmousedOver(Card c)
    {
        _mousedOver[_cards.IndexOf(c)] = false;
    }    

    private bool IsTopMoused(Card c)
    {
        bool onTop = true;

        int endIndex = _cards.IndexOf(c);
        for (int i = _mousedOver.Count-1; i > endIndex; i--)
        {
            if (_mousedOver[i])
                onTop = false;
        }

        return onTop;
    }

    private Card GetTopMoused()
    {
        Card top = null;
        for (int i = 0; i < _mousedOver.Count; i++)
        {
            if (_mousedOver[i])
                top = _cards[i];
        }

        return top;
    }


}

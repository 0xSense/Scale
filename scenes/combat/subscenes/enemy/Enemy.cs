/*
    @author Alexander Venezia (Blunderguy)
    Specific enemy classes will inherit from this one. The resource-based approach would be too painful with all the different animations,
        I think.
*/

namespace Combat;

using Data;
using Godot;
using System;
using System.Collections.Generic;
using Systems.Combat;


public partial class Enemy : Combatant
{
    [Export] private Godot.Collections.Array<CardData> _deck = new();
    private static int SharedCollisionLayer = 3;
    public static uint Bitmask = 0b0100;
    [Export] private int _ID;
    public int UID => _ID;
    [Export] private Godot.Collections.Array<DamageType> _permanentResistances = new();

    public override void _Ready()
    {
        base._Ready();
        SetCollisionMaskValue(1, false);
        SetCollisionMaskValue(SharedCollisionLayer, true);

        List<CardData> converted = new();

        foreach (CardData card in _deck)
        {
            converted.Add(card);
        }

        _deck.Clear();

        Deck.Shuffle(converted);
        foreach (CardData card in converted)
        {
            _internalDeck.AddCard(card);
        }

        foreach (DamageType type in _permanentResistances)
        {
            AddResistance(type, 9999); // If the fight goes on for beyond 9999 rounds, something is very wrong.
        }

        _permanentResistances.Clear();
    }

    public override void BeginTurn()
    {
        GD.Print("Begin enemy turn");
        base.BeginTurn();
    }

    public override int GetMovementPoints()
    {
        return -1;
    }
    
    private void Die()
    {
        QueueFree();
    }

    public override void TakeDamage(DamageType type, int amount, double critModifier, bool isCrit, bool autoResist)
    {
        base.TakeDamage(type, amount, critModifier, isCrit, autoResist);
        if (_isDead)
            Die();
    }

}

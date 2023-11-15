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
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Systems.Combat;


public partial class Enemy : Combatant
{
    [Export] private int _turnDelayTimeStart = 500;
    [Export] private int _turnDelayTimeEnd = 500;
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
        GD.Print("Position: " + Position);
        base.BeginTurn();

        TakeTurn();

    }

    // TODO: Finish
    private List<CardData> DetermineCards()
    {
        List<CardData> cards = null;

        while (_actionPoints > 0)
        {

        }   

        return cards;
    }

    /*
        Current implementation - the enemy cannot reshuffle their discard *mid turn.* If this is desired, either add a "force shuffle" 
        after each card is played OR switch back to the standard Deck.Draw() method. On option would be to draw, say, three card hands
        at the start of each turn and make choices from that. Some enemies could have larger hand sizes than others to differentiate them.
    */
    private async void TakeTurn()
    {
        for (; ; )
        {
            CardData cardToPlay = null;

            await Task.Delay(_turnDelayTimeStart);

            List<CardData> playable = _internalDeck.GetCards((CardData c) =>
            {
                return c.ActionPointCost <= _actionPoints;
            });

            if (playable.Count == 0)
                break;

            int choice = _combatManager.RNG.Next(playable.Count);

            cardToPlay = _internalDeck.DrawSpecific(playable[choice]);

            ICombatant[] targets = null;
            switch (cardToPlay.Target)
            {
                case TargetType.SELF:
                    targets = new ICombatant[] { this };
                    break;
                case TargetType.SINGLE:
                case TargetType.MULTI_TWO:
                case TargetType.MULTI_THREE:
                case TargetType.MULTI_FOUR:
                    targets = new ICombatant[] { _combatManager.Player };
                    break;
                case TargetType.ALL:
                    targets = new ICombatant[] { this, _combatManager.Player };
                    break;
            }

            _combatManager.PlayCard(this, targets, cardToPlay);
            _internalDeck.Discard(cardToPlay);

            FloatingTextFactory.GetInstance().CreateFloatingText("[color=white]" + cardToPlay.Name + "[/color]", Position + Vector2.Up * 100, lifetime: 2500, 300);

        }
        GD.Print("Ending turn");
        await Task.Delay(_turnDelayTimeEnd);
        _internalDeck.ForceShuffle();

        EndTurn();
    }

    public override int GetMovementPoints()
    {
        return 0;
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

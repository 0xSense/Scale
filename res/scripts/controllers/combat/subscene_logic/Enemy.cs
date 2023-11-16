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

    private float CalcAverageDamage(CardData card, ICombatant player)
    {
        const float EXPLOIT_EXPOSED_MULT = 1.25f;
        const float LOW_HEALTH_HEAL_MULT = 2f;
        const float EMERGENCY_HEAL_MULT = 4f;
        float avg = 0;

        foreach (DamageType damageType in card.Damage.Keys)
        {
            float localAvg = 0;
            DamageDice damageDice = card.Damage[damageType];

            localAvg += damageDice.d4 * 2.5f;
            localAvg += damageDice.d6 * 3.5f;
            localAvg += damageDice.d8 * 4.5f;
            localAvg += damageDice.d10 * 5.5f;
            localAvg += damageDice.flat;

            if (player.GetResistances()[damageType] > 0)
            {
                if (player.HasDebuff(DebuffType.EXPOSED))
                    localAvg *= EXPLOIT_EXPOSED_MULT;
                else
                    localAvg *= 0.5f;
            }

            if (damageType == DamageType.HEAL)
            {
                if (_currentHealth + localAvg >= _maxHealth)
                    localAvg = 0f;
                else if (_currentHealth <= MaxHealth/4f)
                    localAvg *= EMERGENCY_HEAL_MULT;  
                else if (_currentHealth <= _maxHealth/2f)
                    localAvg *= LOW_HEALTH_HEAL_MULT;
            }

            avg += localAvg;
        }

        return avg;
    }

    private float CalcBuffEffects(CardData card, ICombatant player)
    {
        const float ARMOR_MULT = 0.25f;
        const float DEBUFF_CURE_MULT = 1.5f;
        const float CRIT_CHANCE_MULT = 2f;
        const float CRIT_DMG_MULT = 2f;
        float value = 0f;

        foreach (Buff buff in card.Buffs)
        {
            switch (buff.Type)
            {
                case BuffType.RESISTANCE:
                value += 1f;
                break;
                case BuffType.ARMOR:
                value += buff.Value*ARMOR_MULT;
                break;
                case BuffType.PURIFY:
                foreach (DebuffType d in _debuffs.Keys)
                {
                    if (_debuffs[d] > 0)
                        value += _debuffs[d] * DEBUFF_CURE_MULT;
                }
                break;
                case BuffType.CRIT_CHANCE_INCREASE:
                value += CRIT_CHANCE_MULT;
                break;
                case BuffType.CRIT_DMG_INCREASE:
                value += CRIT_DMG_MULT;
                break;
            }
        }

        return value;
    }

    private float CalcDebuffEffects(CardData card, ICombatant player)
    {
        const float POISON_MULT = 0.75f;

        float value = 0;

        foreach (Debuff debuff in card.Debuffs)
        {
            switch (debuff.Type)
            {
                case DebuffType.POISONED:
                value += debuff.Duration * POISON_MULT;
                break;
                default:
                value += debuff.Duration * 0.5f;
                break;
            }
        }

        return value;
    }

    private float EvalCard(CardData card, int actionPointsAvailable, ICombatant player)
    {
        float eval = 0f;

        eval += CalcAverageDamage(card, player);
        eval += CalcBuffEffects(card, player);
        eval += CalcDebuffEffects(card, player);

        eval /= card.ActionPointCost;

        return eval;
    }

    // TODO: Finish
    private List<CardData> DetermineCards()
    {
        
        List<CardData> toPlay = new();
        int actionPoints = _actionPoints;

        bool filterFunc(CardData c)
        {
            return c.ActionPointCost <= actionPoints;
        }

        List<CardData> playable = _internalDeck.GetCards(filterFunc);

        while (actionPoints > 0)
        {            
            if (playable.Count == 0)
                break;

            CardData best = null;
            float bestEval = float.NegativeInfinity;
            foreach (CardData card in playable)
            {                
                if (card.ActionPointCost > actionPoints)
                    continue;

                float eval = EvalCard(card, actionPoints, _combatManager.Player);
                // GD.Print("Option: " + card.Name + ". Eval: " + eval);
                if (eval > bestEval)
                {
                    bestEval = eval;
                    best = card;
                }
            }

            // GD.Print("Choice: " + best + ". Eval: " + bestEval);

            if (best != null)
            {
                toPlay.Add(best);
                actionPoints -= best.ActionPointCost;
                playable.Remove(best);
            }
            else
                break;
        }

        
            

        return toPlay;
    }

    /*
        Current implementation - the enemy cannot reshuffle their discard *mid turn.* If this is desired, either add a "force shuffle" 
        after each card is played OR switch back to the standard Deck.Draw() method. On option would be to draw, say, three card hands
        at the start of each turn and make choices from that. Some enemies could have larger hand sizes than others to differentiate them.
    */
    private async void TakeTurn()
    {
        List<CardData> toPlay = DetermineCards();

        // GD.Print("\nTo play:");

        foreach (CardData nextCard in toPlay)
        {
            // GD.Print(nextCard.Name);
            CardData cardToPlay = null;

            await Task.Delay(_turnDelayTimeStart);

            cardToPlay = _internalDeck.DrawSpecific(nextCard);

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
        // GD.Print("Ending turn");
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

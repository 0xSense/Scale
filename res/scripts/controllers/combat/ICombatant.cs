/*
 @author Alexander Venezia (Blunderguy)
*/


/*

    All combatants should fully implement ICombatant. CombatManager will function only with ICombatant-implementing nodes.

    Overview:
    void BeginTurn(): {informs the ICombatant its turn has begun. Reset all movement and action points, apply DOT, update status effects, etc.}
    void TakeDamage(DamageType type, float amount): {reduce health by *damage* after applying resistances/vulnerabilities to *type*}
    bool IsDead() => whether the entity is dead (note that this means you should *not* delete nodes when they are killed. Either set them to be invisible or display them as corpses. The CombatManager
        will react improperly if nodes are abruptly deleted).
    int GetActionPoints() => remaining action points for this turn.
    int GetMovementPoints() => remaining movement points for this turn.
    Deck GetDeck() => returns ICombatant's active deck.
    void BurnActionPoints(int burn): {reduce action points by this amount (called by CombatManager after ICombatant plays a card. ICombatant should not burn its own action points when playing a card for this reason.)}
    void BurnMovementPoints(int burn): {BurnActionPoints(...) equivalent for movement points}

    Message Blunderguy (Alex) about any confusion in the use of this interface.
*/

namespace Systems.Combat;

using System.Collections.Generic;
using Data;

public interface ICombatant
{
    public abstract int GetHealth();
    public abstract void BeginTurn();
    public abstract void EndTurn();
    public abstract void TakeDamage(Data.DamageType type, int amount, double critModifier, bool isCrit, bool autoResist);
    public abstract bool IsDead();
    public abstract int GetActionPoints();
    public abstract int GetMovementPoints();
    public abstract Deck GetDeck();
    public abstract void BurnActionPoints(int burn); // If this function reduces action points below zero, set them to zero.
    public abstract void BurnMovementPoints(int burn); // If this function reduces movement points below zero, set them to zero.
    public abstract int GetArmor();
    public abstract void AddArmor(int armor);
    public abstract void AddResistance(DamageType resistance, int turns);
    public abstract int GetCritChance();
    public abstract double GetCritModifier();
    public Dictionary<Data.DamageType, int> GetResistances();
    public void StartFight();

    public void ApplyBuff(Buff buff);
    public void ApplyDebuff(Debuff debuff);
    public bool HasDebuff(DebuffType type);
    public void DrawCards(int n);
    public void DiscardCards(int n);
    public void ReturnCards(int n);
    public void EndFight(EndState result);
    
}
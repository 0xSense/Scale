/*
 @author Alexander Venezia (Blunderguy)
*/

/*
    ** NOTE: This class is INCOMPLETE. Any behaviors you need which it does not currently provide should be immediately send to Blunderguy (Alex). Please do not implement a hacky workaround instead. **
    Also, as I (Blunderguy) have not been able to thoroughly test any of this backend combat behavior without a complementary frontend, bugs are a probability. Please inform me of any possible or certain
    bugs you encounter as soon as possible.

    -- -- -- -- -- -- -- -- --

    Follows singleton pattern. Call NewFight() every time a new fight begins.
    CombatManager stores both the player and enemy nodes as implementations of the ICombatant interface. When NewFight() is called, all player and enemy combatants should be passed.
    If the feature to add additional combatants once combat has started (I.E. reinforcements) is required, message Blunderguy (Alex).

    Nomenclature:
    Within this class documentation, the term CMInstance refers to the singleton instance of the CombatManager class.

    Overview:

    CombatManager GetInstance() => CMInstance
    void NewFight(ICombatant player, ICombatant[] enemies): {resets CMInstance and initiates a new fight}
    bool PlayCard(ICombatant cardPlayer, ICombatant[] targets, CardData card): {effects behavior of card on targets} => returns whether card was succesfully played
    ^ IF PlayCard(. . .) EVER RETURNS FALSE IT IS BECAUSE YOU ATTEMPTED TO PLAY AN ILLEGAL CARD OR BECAUSE OF A BUG IN CombatManager. If you are confident it's a bug in CombatManager, contact Blunderguy. ^
    void EndTurn(ICombatant turnEnder): {Ends active player's turn if active player matches parameter}

    Architecture:
    
    CombatManager is primarily driven by its ICombatants. When an ICombatant is ready to perform an action, it will call CMInstance.PlayCard(. . .). 
    CombatManager will determine whether it is a legal move based on the player's remaining action/movement points as well as any other factors. If it is determined to be illegal, the function
    will return false and no action will be performed. If it is legal, it will return true and perform the action defind in the CardData parameter.
    An ICombatant may also end its turn at any point by calling CMInstance.EndTurn(. . .). The CMInstance will then inform the next ICombatant in the initiative order that it is time for its move via the ICombatant's BeginTurn() function.
*/

namespace Systems.Combat;

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Combat;
using Data;
using Godot;

public enum EndState
{
    VICTORY,
    DEFEAT
}

public class CombatManager
{
    private Random _random;    
    public Random RNG => _random;
    private ICombatant _player;
    public ICombatant Player => _player;
    private ICombatant[] _enemies;
    public ICombatant[] Enemies => _enemies;

    private ICombatant[] _moveOrder;
    public ICombatant[] MoveOrder => _moveOrder;
    private int _currentToMove;
    public ICombatant ToMove => _moveOrder[_currentToMove];

    private static CombatManager _instance;
    private bool _isOver;
    
    public static CombatManager GetInstance()
    {
        if (_instance == null)
            _instance = new CombatManager();
        return _instance;
    }

    public int GetRemainingEnemies()
    {
        int remaining = 0;

        foreach (ICombatant enemy in _enemies)
        {
            if (!enemy.IsDead())
                remaining++;
        }
        
        return remaining;
    }

    public void NewFight(ICombatant player, ICombatant[] enemies)
    {
        GD.Print("Fight has begun");

        _isOver = false;

        _player = player;
        _enemies = enemies;

        _moveOrder = new ICombatant[_enemies.Length + 1]; // +1 for player
        _moveOrder[0] = _player;
        
        int i = 0;
        foreach (ICombatant c in enemies)
            _moveOrder[++i] = c;        

        _currentToMove = 0;
        _player.StartFight();

        _player.BeginTurn();
    }

    public bool PlayCard(ICombatant cardPlayer, ICombatant[] targets, CardData card)
    {
        if (card.Target == TargetType.SELF && cardPlayer == _player)
        {
            targets = new ICombatant[]{_player};
        }
        else if (card.Target == TargetType.ALL)
        {
            targets = _moveOrder;
        }

        if (cardPlayer.GetActionPoints() < card.ActionPointCost || cardPlayer.GetMovementPoints() < card.MovementPointCost)
            throw new Exception("TOO FEW POINTS " + card.Name);
        
        if (!VerifyTargeting(cardPlayer, targets, card))
            throw new Exception ("TARGETING INVALID " + card.Name);

        


        /*
        No real need for this.
        if (!VerifyDeck(cardPlayer.GetDeck(), card))
            return false;
        */

        // Put any other necessary verification here
        // . . .

        // Card has been verified legal; apply effects

        foreach (ICombatant target in targets)
        {
            foreach (DamageType dt in card.Damage.Keys)
            {
                bool isAutoResisted = (cardPlayer.HasDebuff(DebuffType.TRAUMATIZED) && !IsPhysicalDamage(dt)) ||
                        (cardPlayer.HasDebuff(DebuffType.CRIPPLED) && IsPhysicalDamage(dt))
                    ;

                target.TakeDamage(dt, Roll(card.Damage[dt]), cardPlayer.GetCritModifier(), RollForCrit(cardPlayer.GetCritChance()), autoResist:isAutoResisted);
            }

            foreach (Buff b in card.Buffs)
            {
                target.ApplyBuff(b);
            }

            foreach (Debuff d in card.Debuffs)
            {
                target.ApplyDebuff(d);
            }

            foreach (DrawEffect de in card.DrawEffects.Keys)
            {
                switch (de)
                {
                    case DrawEffect.DRAW:
                    cardPlayer.DrawCards(card.DrawEffects[de]);
                    break;
                    case DrawEffect.RETURN:
                    cardPlayer.ReturnCards(card.DrawEffects[de]);
                    break;
                    case DrawEffect.DISCARD:
                    cardPlayer.DiscardCards(card.DrawEffects[de]);
                    break;
                    case DrawEffect.GRAB:
                    throw new NotImplementedException("GRAB not implemented yet.");
                    default:
                    throw new Exception("Invalid draw effect.");
                }
            }
        }

        cardPlayer.BurnActionPoints(card.ActionPointCost);
        cardPlayer.BurnMovementPoints(card.MovementPointCost);

        // Remove card from playing combatant's deck here
        // . . .

        return true; // TODO: Complete function
    }

    private bool IsPhysicalDamage(DamageType damage)
    {
        switch (damage)
        {
            case DamageType.SHARP:
            return true;
            case DamageType.BLUNT:
            return true;
            case DamageType.PIERCING:
            return true;
            default:
            return false;
        }
    }

    private bool RollForCrit(int percentChance)
    {
        return (RNG.Next(100)+1) < percentChance;
    }

    public void EndTurn(ICombatant current)
    {
        if (_moveOrder[_currentToMove] == current)
            AdvanceInitiative();
    }

    private bool VerifyTargeting(ICombatant cardPlayer, ICombatant[] targets, CardData card)
    {
        switch (card.Target)
        {
            case TargetType.SINGLE:
                if (targets.Length != 1)
                    return false;
                break;
            case TargetType.SELF:
                if (targets[0] != cardPlayer)
                {
                    GD.Print("ERR: " + targets.Length);
                    GD.Print(targets[0] == _player);

                    return false;
                }
                break;
            case TargetType.MULTI_TWO:
                if (targets.Length > 2)
                    return false;
                break;
            case TargetType.MULTI_THREE:
                if (targets.Length > 3)
                        return false;
                break;
            case TargetType.ALL:
                break;
        }

        return true;
    }

    private bool VerifyDeck(Deck deck, CardData card)
    {
        return deck.GetCount(card) > 0;
    }

    private void AdvanceInitiative()
    {
        if (_isOver)
            return;
            
        if (_player.IsDead())
        {
            _isOver = true;
            _player.EndFight(EndState.DEFEAT);
            return;
        }

        int _endingTurn = _currentToMove;
        while (true)
        {
            if (_currentToMove >= _moveOrder.Length-1)
                _currentToMove = -1;
            if (!_moveOrder[++_currentToMove].IsDead())
                break;
        }

        if (_endingTurn != _currentToMove)
            _moveOrder[_currentToMove].BeginTurn();
        else
        {
            // Battle is over and player is alive
            _isOver = true;
            _player.EndFight(EndState.VICTORY);
        }
    }

    private int Roll(DamageDice dice)
    {
        int damageTotal = 0;

        for (int i = 0; i < dice.d4; i++)        
            damageTotal += _random.Next(4) + 1;
        for (int i = 0; i < dice.d6; i++)        
            damageTotal += _random.Next(6) + 1;
        for (int i = 0; i < dice.d8; i++)        
            damageTotal += _random.Next(8) + 1;
        for (int i = 0; i < dice.d10; i++)        
            damageTotal += _random.Next(10) + 1;
        
        damageTotal += dice.flat;

        return damageTotal; 
    }

    private CombatManager() 
    {
        _random = new();

    }    
}
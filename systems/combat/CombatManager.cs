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
using Data;
using Systems.Combat;

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

    private static CombatManager _instance;
    
    public static CombatManager GetInstance()
    {
        if (_instance == null)
            _instance = new CombatManager();
        return _instance;
    }

    public void NewFight(ICombatant player, ICombatant[] enemies)
    {
        _player = player;
        _enemies = enemies;

        _moveOrder = new ICombatant[_enemies.Length + 1]; // +1 for player
        _moveOrder[0] = _player;
        
        int i = 0;
        foreach (ICombatant c in enemies)
            _moveOrder[++i] = c;        

        _currentToMove = 0;
    }

    public bool PlayCard(ICombatant cardPlayer, ICombatant[] targets, CardData card)
    {
        if (cardPlayer.GetActionPoints() <= card.ActionPointCost || cardPlayer.GetMovementPoints() <= card.MovementPointCost)
            return false;
        
        if (!VerifyTargeting(cardPlayer, targets, card))
            return false;

        if (!VerifyDeck(cardPlayer.GetDeck(), card))
            return false;

        // Put any other necessary verification here
        // . . .

        // Card has been verified legal; apply effects

        foreach (ICombatant target in targets)
        {
            foreach (DamageType dt in card.Damage.Keys)
            {
                target.TakeDamage(dt, Roll(card.Damage[dt]));
            }
        }

        // Remove card from playing combatant's deck here
        // . . .

        return true; // TODO: Complete function
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
                    return false;
                break;
            case TargetType.MULTI_TWO:
                if (targets.Length != 2)
                    return false;
                break;
            case TargetType.MULTI_THREE:
                if (targets.Length != 3)
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
        while (true)
        {
            if (_currentToMove >= _moveOrder.Length)
                _currentToMove = 0;
            if (!_moveOrder[++_currentToMove].IsDead())
                break;
        }
    }

    private int Roll(DamageDice dice)
    {
        int damageTotal = 0;

        for (int i = 0; i < dice.d4; i++)        
            damageTotal += _random.Next(1, 4);
        for (int i = 0; i < dice.d6; i++)        
            damageTotal += _random.Next(1, 6);
        for (int i = 0; i < dice.d8; i++)        
            damageTotal += _random.Next(1, 8);
        for (int i = 0; i < dice.d10; i++)        
            damageTotal += _random.Next(1, 10);
        
        damageTotal += dice.flat;

        return damageTotal; 
    }

    private CombatManager() 
    {
        _random = new();

    }    
}
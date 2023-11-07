/*
    Specific enemy classes will inherit from this one. The resource-based approach would be too painful with all the different animations,
        I think.
*/

namespace Combat;

using Data;
using Godot;
using System;
using System.Collections.Generic;
using Systems.Combat;


public partial class Enemy : Sprite2D, Systems.Combat.ICombatant
{
    protected int _maxHealth = 10;
    protected int _startingActionPoints = 2;
    protected int _defaultCritChance = 10;
    protected double _defaultCritModifier = 2.0;

    protected int _currentHealth;
    protected int _armor;
    protected int _actionPoints;
    protected int _critChance;
    protected double _critModifier;
    protected bool _isDead;
    protected bool _isTurn;

    protected Systems.Combat.Deck _internalDeck;
    protected Dictionary<Data.DamageType, int> _resistances;

    public void AddArmor(int armor)
    {
        _armor += armor;
    }

    public void AddResistance(DamageType resistance, int turns)
    {
        _resistances[resistance] += turns;
    }

    public virtual void BeginTurn()
    {
        _isTurn = true;        
    }

    public void BurnActionPoints(int burn)
    {
        _actionPoints -= burn;
    }

    public void BurnMovementPoints(int burn)
    {}

    public int GetActionPoints()
    {
        return _actionPoints;
    }

    public int GetArmor()
    {
        return _armor;
    }

    public int GetCritChance()
    {
        return _critChance;
    }

    public double GetCritModifier()
    {
        return _critModifier;
    }

    public Deck GetDeck()
    {
        return _internalDeck;
    }

    public int GetHealth()
    {
        return _currentHealth;
    }

    public int GetMovementPoints()
    {
        return -1;
    }

    public Dictionary<DamageType, int> GetResistances()
    {
        return _resistances;
    }

    public bool IsDead()
    {
        return _isDead;
    }

    public void TakeDamage(DamageType type, int amount, double critModifier, bool isCrit)
    {
        if (type == DamageType.SHARP || type == DamageType.BLUNT)
        {
            int dmg_temp = amount;
            amount -= _armor;
            amount = Math.Max(0, amount);
            _armor -= dmg_temp;
            _armor = Math.Max(0, _armor);
        }        

        int isCritInteger = isCrit ? 1 : 0;
 
        int isResisted = (_resistances[type] > 0) ? 1 : 0;

        int damage = (int)(amount * (1 + critModifier * isCritInteger) * (1 + isResisted * (0.5 + (0.25 * isCritInteger))));
        _currentHealth -= damage;
        if (_currentHealth < 0)
            _isDead = true;
    }

}

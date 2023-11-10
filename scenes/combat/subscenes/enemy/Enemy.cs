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


public partial class Enemy : Area2D, Systems.Combat.ICombatant
{
    private static int SharedCollisionLayer = 3;
    public static uint Bitmask = 0b0100;
    [Export] private int _ID;
    public int UID => _ID;
    [Export] private int _maxHealth = 10;
    [Export] private int _startingActionPoints = 2;
    [Export] private int _defaultCritChance = 10;
    [Export] private double _defaultCritModifier = 2.0;

    [Export] private Godot.Collections.Array<CardData> _deck = new();
    [Export] private Godot.Collections.Array<DamageType> _permanentResistances = new();

    private int _currentHealth;
    private int _armor;
    private int _actionPoints;
    private int _critChance;
    private double _critModifier;
    private bool _isDead;
    private bool _isTurn;

    private Systems.Combat.Deck _internalDeck;
    private Dictionary<Data.DamageType, int> _resistances;

    public override void _Ready()
    {
        SetCollisionMaskValue(1, false);
        SetCollisionMaskValue(SharedCollisionLayer, true);

        _internalDeck = new();
        _resistances = new();
        _currentHealth = _maxHealth;

        List<CardData> converted = new();

        foreach (DamageType type in Enum.GetValues(typeof(Data.DamageType)))
        {
            _resistances.Add(type, 0);
        }

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

    public void StartFight()
    {
        
    }

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
        GD.Print("Begin enemy turn");
        _isTurn = true;        
        _actionPoints = _startingActionPoints;
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

    private void Die()
    {
        QueueFree();
    }

    public void TakeDamage(DamageType type, int amount, double critModifier, bool isCrit)
    {
        if (type == DamageType.HEAL)
        {
            _currentHealth += amount;
            _currentHealth = Math.Max(_currentHealth, _maxHealth);
            FloatingTextFactory.GetInstance().CreateFloatingText("[color=green]+" + amount + "[/color]", GlobalPosition + Vector2.Up * 150);
            return;
        }
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
        {
           _isDead = true;
           Die();
        }

        FloatingTextFactory.GetInstance().CreateFloatingText("[color=red]-" + damage + "[/color]", GlobalPosition + Vector2.Up * 150);
    }

}

/*
 @author Alexander Venezia (Blunderguy)
*/


namespace Combat;

using Data;
using Godot;
using System;
using System.Collections.Generic;
using Systems.Combat;


public partial class Combatant : Area2D, ICombatant
{
    [Export] protected int _maxHealth;
    [Export] protected int _startingActionPoints = 3;
    [Export] protected int _startingMovementPoints = 1;
    [Export] protected int _defaultCritChance = 10; // Out of 100
    [Export] protected double _defaultCritModifier = 2.0;
    [Export] protected int _startingArmor;

    public int MaxHealth => _maxHealth;
    protected int _currentHealth;
    public int CurrentHealth => _currentHealth;
    protected int _armor;
    protected int _movementPoints;
    protected int _actionPoints;
    protected int _critChance;
    protected double _critModifier;

    protected bool _isDead;
    protected bool _isTurn;

    protected Systems.Combat.Deck _internalDeck = new();

    protected Dictionary<Data.DamageType, int> _resistances;
    protected Dictionary<BuffType, int> _buffs;
    protected Dictionary<DebuffType, int> _debuffs;
    protected CombatManager _combatManager;

    public override void _Ready()
    {
        _combatManager = CombatManager.GetInstance();
        _currentHealth = _maxHealth;
        _isDead = false;
        _internalDeck = new();
        _resistances = new();
        _buffs = new();
        _debuffs = new();
        _armor = _startingArmor;
        _critChance = _defaultCritChance;
        _critModifier = _defaultCritModifier;

        foreach (Data.DamageType type in Enum.GetValues(typeof(Data.DamageType)))
        {
            _resistances.Add(type, 0);
        }

    }

    public virtual void StartFight()
    {

    }

    public virtual void BeginTurn()
    {
        _isTurn = true;
        _movementPoints = _startingMovementPoints;
        _actionPoints = _startingActionPoints;

        foreach (Data.DamageType resistance in _resistances.Keys)
        {
            _resistances[resistance]--;
            _resistances[resistance] = Math.Max(0, _resistances[resistance]);
        }

        if (_debuffs.ContainsKey(DebuffType.POISONED) && _debuffs[DebuffType.POISONED] > 0)
            TakeDamage(DamageType.POISON, 1, 1, false, false);

        foreach (BuffType buff in _buffs.Keys)
        {
            _buffs[buff]--;
            _buffs[buff] = Math.Max(0, _buffs[buff]);
        }

        foreach (DebuffType debuff in _debuffs.Keys)
        {
            _debuffs[debuff]--;
            _debuffs[debuff] = Math.Max(0, _debuffs[debuff]);
        }        
            

        // TODO: Fill out function
    }

    public virtual void BurnActionPoints(int burn)
    {
        _actionPoints -= burn;
        _actionPoints = Math.Max(_actionPoints, 0);
    }

    public virtual void BurnMovementPoints(int burn)
    {
        _movementPoints -= burn;
        _movementPoints = Math.Max(_movementPoints, 0);
    }

    public virtual void SetHP(int hp)
    {
        _currentHealth = hp;
    }

    public virtual void TakeDamage(Data.DamageType type, int amount, double critModifier, bool isCrit, bool autoResist)
    {
        if (type == DamageType.HEAL)
        {            
            _currentHealth += amount;
            _currentHealth = Math.Max(_currentHealth, _maxHealth);
            FloatingTextFactory.GetInstance().CreateFloatingCardText(true, isCrit, false, false, 0, amount, GlobalPosition);
            return;
        }

        int armorDamage = 0;

        if ((type == DamageType.SHARP || type == DamageType.BLUNT) && !HasDebuff(DebuffType.EXPOSED))
        {
            int dmgTemp = amount;
            amount -= _armor;
            amount = Math.Max(0, amount);
            armorDamage = dmgTemp;
            armorDamage = Math.Min(armorDamage, _armor);
            _armor -= armorDamage;            
            _armor = Math.Max(0, _armor);
        }        

        int isCritInteger = isCrit ? 1 : 0;
 
        int isResisted = ((_resistances[type] > 0) || autoResist) ? 1 : 0;

        if (HasDebuff(DebuffType.EXPOSED))
            isResisted = 0;

        int damage = (int)(amount * (1 + critModifier * isCritInteger) * (1 + isResisted * (-0.5 + (0.25 * isCritInteger))));
        _currentHealth -= damage;
        if (_currentHealth < 0)
            _isDead = true;

        FloatingTextFactory.GetInstance().CreateFloatingCardText(false, isCrit, type==DamageType.POISON, isResisted > 0, armorDamage, damage, GlobalPosition);
    }

    public virtual void ApplyBuff(Buff buff)
    {
        switch (buff.Type)
        {
            case BuffType.ARMOR:
            _armor += buff.Value;
            return;
            case BuffType.RESISTANCE:
            _resistances[buff.ResistanceType] += buff.Value;
            return;          
            case BuffType.PURIFY:
            _debuffs = new();
            return;  
        }

        if (_buffs.ContainsKey(buff.Type))
            _buffs[buff.Type] += buff.Value;
        else
            _buffs.Add(buff.Type, buff.Value);
    }

    public virtual void ApplyDebuff(Debuff debuff)
    {
        if (debuff.Type == DebuffType.WITHER)
            _buffs = new();
            
        if (_debuffs.ContainsKey(debuff.Type))
            _debuffs[debuff.Type] += debuff.Duration;
        else
            _debuffs.Add(debuff.Type, debuff.Duration);
    }

    public virtual void AddArmor(int armor)
    {
        _armor += armor;
    }

    public virtual void AddResistance(DamageType resistance, int turns)
    {        
        _resistances[resistance] += turns;
    }

    public virtual int GetActionPoints()
    {
        return _actionPoints;
    }

    public virtual int GetMovementPoints()
    {
        return _movementPoints;
    }

    public virtual Systems.Combat.Deck GetDeck()
    {        
        return _internalDeck;
    }

    public virtual bool IsDead()
    {
        return _isDead;
    }

    public virtual int GetHealth()
    {
        return _currentHealth;
    }

    public virtual int GetArmor()
    {
        return _armor;
    }

    public virtual Dictionary<Data.DamageType, int> GetResistances()
    {
        return _resistances;
    }

    public virtual int GetCritChance()
    {
        if (_buffs.ContainsKey(BuffType.CRIT_CHANCE_INCREASE) && _buffs[BuffType.CRIT_CHANCE_INCREASE] > 0)
            return _critChance * 2;
        return _critChance;
    }

    public virtual double GetCritModifier()
    {
        if (_buffs.ContainsKey(BuffType.CRIT_DMG_INCREASE) && _buffs[BuffType.CRIT_DMG_INCREASE] > 0)
            return _critModifier + 1;
        return _critModifier;
    }

    public virtual bool HasDebuff(DebuffType type)
    {
        if (_debuffs.ContainsKey(type))
            return _debuffs[type] > 0;
        return false;
    }

    public virtual void EndTurn()
    {
        _isTurn = false;
        _combatManager.EndTurn(this);        
    }

    public virtual void DrawCards(int n)
    {
        
    }

    public virtual void DiscardCards(int n)
    {
        
    }

    public virtual void ReturnCards(int n)
    {
        
    }

    public virtual void EndFight(EndState result) {}

}

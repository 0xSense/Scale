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
    public Dictionary<BuffType, int> Buffs => _buffs;
    public Dictionary<DebuffType, int> Debuffs => _debuffs;
    protected CombatManager _combatManager;

    private AudioStreamPlayer _dmgPhysical;    
    private AudioStreamPlayer _critPhysical;
    private AudioStreamPlayer _dmgMagic;
    private AudioStreamPlayer _critMagic;

    public override void _Ready()
    {
        _dmgPhysical = (AudioStreamPlayer)GetTree().Root.GetChild(0).GetNode("Combat/Audio/DmgPhysical");
        _critPhysical = (AudioStreamPlayer)GetTree().Root.GetChild(0).GetNode("Combat/Audio/DmgPhysical");
        _dmgMagic = (AudioStreamPlayer)GetTree().Root.GetChild(0).GetNode("Combat/Audio/DmgPhysical");
        _critMagic = (AudioStreamPlayer)GetTree().Root.GetChild(0).GetNode("Combat/Audio/DmgPhysical");


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

        foreach (BuffType buff in _buffs.Keys)
        {
            _buffs[buff]--;
            _buffs[buff] = Math.Max(0, _buffs[buff]);
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
        GD.Print("\nHIT receieved");
        GD.Print(type);
        GD.Print(amount);
        GD.Print(critModifier);
        GD.Print(isCrit);
        GD.Print(autoResist);

        if (type == DamageType.HEAL)
        {            
            _currentHealth += amount;
            _currentHealth = Math.Min(_currentHealth, _maxHealth);
            FloatingTextFactory.GetInstance().CreateFloatingCardText(true, false, false, false, 0, amount, GlobalPosition);
            return;
        }

        float armorDamage = 0;    

        int isCritInteger = isCrit ? 1 : 0;
 
        int isResisted = ((_resistances[type] > 0) || autoResist) ? 1 : 0;

        float amountPostCrit = (float)(amount * (1.0 + (critModifier-1.0) * isCritInteger));
        float armorDamageBypass = 0;

        if ((type == DamageType.SHARP || type == DamageType.BLUNT))
        {
            /*
            amount -= _armor;
            amount = Math.Max(0, amount);
            armorDamage = dmgTemp;
            armorDamage = Math.Min(armorDamage, _armor);
            _armor -= armorDamage;            
            _armor = Math.Max(0, _armor);
            */

            armorDamage = Mathf.Min(amountPostCrit, (float)_armor);
            _armor -= (int)armorDamage;
            armorDamageBypass = armorDamage * (1.0f + (-0.5f + (0.25f * isCritInteger)));
            amountPostCrit -= armorDamage;

            Vector2 textPos = Position + ((Node2D)(GetNode("HealthBar/Armor"))).Position + Vector2.Down * 100;
            GD.Print("Pos: " + textPos);
            FloatingTextFactory.GetInstance().CreateFloatingText("[color=#666666]" + armorDamage.ToString() + "[/color]", textPos);
        }    

        if (HasDebuff(DebuffType.EXPOSED))
            isResisted = 0;

        int damage = (int)(amountPostCrit * (1.0 + isResisted * (-0.5 + (0.25 * isCritInteger))) + armorDamageBypass);
        _currentHealth -= damage;
        if (_currentHealth <= 0)
            _isDead = true;

        FloatingTextFactory.GetInstance().CreateFloatingCardText(false, isCrit, type==DamageType.POISON, isResisted > 0, 0, damage, GlobalPosition);

        GD.Print("\nFinal damage:");
        GD.Print("Final applied: " + damage);
        GD.Print("Bypassed armor: " + armorDamageBypass);

        if (type == DamageType.SHARP || type == DamageType.BLUNT || type == DamageType.PIERCING)
        {
            if (isCrit)
                _critPhysical.Play();
            else
                _dmgPhysical.Play();
        }
        else
        {
            if (isCrit)
                _critMagic.Play();
            else
                _dmgMagic.Play();
        }
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
            _buffs[buff.Type] = Math.Max(buff.Value, _buffs[buff.Type]);
        else
            _buffs.Add(buff.Type, buff.Value);
    }

    public virtual void ApplyDebuff(Debuff debuff)
    {
        if (debuff.Type == DebuffType.WITHER)
            _buffs = new();
            
        if (_debuffs.ContainsKey(debuff.Type))
            _debuffs[debuff.Type] = Math.Max(debuff.Duration, _debuffs[debuff.Type]);
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
        if (_debuffs.ContainsKey(DebuffType.POISONED) && _debuffs[DebuffType.POISONED] > 0)
            TakeDamage(DamageType.POISON, 1, 1, false, false);

        foreach (DebuffType debuff in _debuffs.Keys)
        {
            _debuffs[debuff]--;
            _debuffs[debuff] = Math.Max(0, _debuffs[debuff]);
        }    

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

 /*
 @author Alexander Venezia (Blunderguy)
*/

namespace Combat;

using Data;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Systems.Combat;

public enum PlayerState
{
    SELECTING_CARD,
    SELECTING_TARGETS,
}

public partial class Player : Sprite2D, Systems.Combat.ICombatant
{
    [Export] Hand _hand;
    [Export] RichTextLabel _targetLabel;
    [Export] private int _maxHealth;
    [Export] private int _startingActionPoints = 3;
    [Export] private int _startingMovementPoints = 1;
    [Export] private int _defaultCritChance = 10; // Out of 100
    [Export] private double _defaultCritModifier = 2.0;

    public int MaxHealth => _maxHealth;
    private int _currentHealth;
    public int CurrentHealth => _currentHealth;
    private int _armor;
    private int _movementPoints;
    private int _actionPoints;
    private int _critChance;
    private double _critModifier;

    private bool _isDead;
    private bool _isTurn;

    private Systems.Combat.Deck _internalDeck;

    private Dictionary<Data.DamageType, int> _resistances;

    private PlayerState _state;
    public PlayerState State => _state;

    private Card _currentlyTargeting;
    private List<ICombatant> _targeted = new();

    public override void _Ready()
    {
        _currentHealth = _maxHealth;
        _isDead = false;
        _internalDeck = new();
        _resistances = new();
        _critChance = _defaultCritChance;
        _critModifier = _defaultCritModifier;

        foreach (Data.DamageType type in Enum.GetValues(typeof(Data.DamageType)))
        {
            _resistances.Add(type, 0);
        }

        _armor = 0;
        _state = PlayerState.SELECTING_CARD;
    }

    public void BeginTurn()
    {
        GD.Print("Begin player turn");
        _isTurn = true;
        _movementPoints = _startingMovementPoints;
        _actionPoints = _startingActionPoints;

        foreach (Data.DamageType resistance in _resistances.Keys)
        {
            _resistances[resistance]--;
            _resistances[resistance] = Math.Max(0, _resistances[resistance]);
        }

        // TODO: Fill out function
    }

    public override void _PhysicsProcess(double delta)
    {
        void endTargeting()
        {
            _currentlyTargeting = null;
            _hand.Unfreeze();
            _targetLabel.Visible = false;
            _state = PlayerState.SELECTING_CARD;
            _targeted = new();
        }
        if (!_isTurn)
            return;        

        if (Input.IsActionJustPressed("Select"))
        {
            switch (_state)
            {
                case PlayerState.SELECTING_CARD:
                    _currentlyTargeting = _hand.GetSelectedCard();
                    if (_currentlyTargeting != null)
                    {
                        _state = PlayerState.SELECTING_TARGETS;
                        _targetLabel.Visible = true;
                        _hand.Freeze();
                        _currentlyTargeting.ZIndex += 99;
                    }

                    break;     
                case PlayerState.SELECTING_TARGETS:
                    Enemy clicked = GetEnemyUnderMouse();
                    if (clicked != null)
                    {
                        _targeted.Add(clicked);
                    }

                    if (IsTargetingValid())
                    {
                        Vector2 position = _currentlyTargeting.GlobalPosition;
                        _hand.RemoveCard(_currentlyTargeting);
                        _currentlyTargeting.Position = position;
                        GetParent().AddChild(_currentlyTargeting);

                        _currentlyTargeting.BeginPlayAnimation();
                        
                        endTargeting();
                    }
                    break;           
            }
        }

        if (Input.IsActionJustPressed("Deselect") && _state == PlayerState.SELECTING_TARGETS)
        {
            endTargeting();
        }
    }

    /* Enemies are not supposed to overlap, so no need for z-checking.*/
    private Enemy GetEnemyUnderMouse()
    {
        Enemy enemy = null;

        Vector2 mousePos = GetGlobalMousePosition();
        PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
        PhysicsPointQueryParameters2D query = new();
        query.CollideWithAreas = true;
        query.Position = mousePos;
        query.CollisionMask = Enemy.Bitmask;
        var hits = spaceState.IntersectPoint(query);

        if (hits.Count == 0)
            return null;
            
        enemy = (Enemy)hits.ElementAt(0)["collider"];

        return enemy;
    }

    private bool IsTargetingValid()
    {
        switch (_currentlyTargeting.Data.Target)
        {
            case TargetType.SELF:
            return true;
            case TargetType.SINGLE:
            return _targeted.Count == 1;
            case TargetType.MULTI_TWO:
            return _targeted.Count == 2;
            case TargetType.MULTI_THREE:
            return _targeted.Count == 3;
            case TargetType.MULTI_FOUR:
            return true;
            case TargetType.ALL:
            return true;
            default:
            return false;
        }
    }

    public void BurnActionPoints(int burn)
    {
        _actionPoints -= burn;
        _actionPoints = Math.Max(_actionPoints, 0);
    }

    public void BurnMovementPoints(int burn)
    {
        _movementPoints -= burn;
        _movementPoints = Math.Max(_movementPoints, 0);
    }

    public void SetHP(int hp)
    {
        _currentHealth = hp;
    }

    public void TakeDamage(Data.DamageType type, int amount, double critModifier, bool isCrit)
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

    public void AddArmor(int armor)
    {
        _armor += armor;
    }

    public void AddResistance(DamageType resistance, int turns)
    {        
        _resistances[resistance] += turns;
    }

    public int GetActionPoints()
    {
        return _actionPoints;
    }

    public int GetMovementPoints()
    {
        return _movementPoints;
    }

    public Systems.Combat.Deck GetDeck()
    {
        return _internalDeck;
    }

    public bool IsDead()
    {
        return _isDead;
    }

    public int GetHealth()
    {
        return _currentHealth;
    }

    public int GetArmor()
    {
        return _armor;
    }

    public Dictionary<Data.DamageType, int> GetResistances()
    {
        return _resistances;
    }

    public int GetCritChance()
    {
        return _critChance;
    }

    public double GetCritModifier()
    {
        return _critModifier;
    }

}

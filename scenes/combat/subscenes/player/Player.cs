 /*
 @author Alexander Venezia (Blunderguy)
*/

namespace Combat;

using Data;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Systems.Combat;

public enum PlayerState
{
    SELECTING_CARD,
    SELECTING_TARGETS,
}

public partial class Player : Combatant
{
    [Export] Hand _hand;
    [Export] RichTextLabel _targetLabel;

    private PlayerState _state;
    public PlayerState State => _state;

    private Card _currentlyTargeting;
    private List<ICombatant> _targeted = new();

    public override void _Ready()
    {
        base._Ready();
        _state = PlayerState.SELECTING_CARD;
        GD.Print(_state);
    }

    private void SyncDeck()
    {        
        _internalDeck = MasterDeck.PlayerDeck;
    }

    public override void StartFight()
    {
        base.StartFight();
        SyncDeck();
        _internalDeck.ForceShuffle();
        _hand.DrawOpeningHand(_internalDeck);
    }

    public override void BeginTurn()
    {
        base.BeginTurn();
        GD.Print("Begin player turn");
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

                        PlayCard(_targeted.ToArray(), _currentlyTargeting);                                            
                        
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

    private async void PlayCard(ICombatant[] targets, Card card)
    {
        await card.BeginPlayAnimation();
        _combatManager.PlayCard(this, targets, card.Data);
        _internalDeck.Discard(card.Data);
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
            return _targeted.Count == 2 || (_targeted.Count >= _combatManager.GetRemainingEnemies());
            case TargetType.MULTI_THREE:
            return _targeted.Count == 3 || (_targeted.Count >= _combatManager.GetRemainingEnemies());
            case TargetType.MULTI_FOUR:
            return true;
            case TargetType.ALL:
            return true;
            default:
            return false;
        }
    }


    public async void OnEndTurnButtonInput(Viewport node, InputEvent e, int shapeID)
    {
        if (e.IsActionPressed("Select"))
        {
            if (_isTurn)
            {
                _isTurn = false;
                await Task.Delay(1000);
                this.EndTurn();
            }
        }
    }

}

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
    DISCARDING_CARDS,
    RETURNING_CARDS,
    GAME_OVER
}

public partial class Player : Combatant
{
    [Export] Hand _hand;
    [Export] RichTextLabel _targetLabel;
    [Export] Label _actionPointLabel;
    [Export] Label _movementPointLabel;
    [Export] Node2D _deck;
    [Export] Node2D _discard;

    private AudioStreamPlayer _drawSound;
    private AudioStreamPlayer _selectSound;
    private AudioStreamPlayer _dropSound;

    private PlayerState _state;
    public PlayerState State => _state;

    private Card _currentlyTargeting;
    private AnimatedSprite2D _sprite;
    private List<ICombatant> _targeted = new();
    private int _toDiscard; // Refers to either discarding or returning
    private bool _firstTurn;


    public override void _Ready()
    {
        GD.Print("Ready");
        _firstTurn = true;
        base._Ready();
        _toDiscard = 0;
        _state = PlayerState.SELECTING_CARD;
        _targetLabel.Visible = false;
        _sprite = (AnimatedSprite2D)GetNode<AnimatedSprite2D>("Player");
        _internalDeck.OnDiscardChange += OnDiscardChange;

        _drawSound = (AudioStreamPlayer)GetNode("DrawSound");
        _selectSound = (AudioStreamPlayer)GetNode("SelectSound");
        _dropSound = (AudioStreamPlayer)GetNode("DropSound");

    }

    private void OnDiscardChange()
    {
        GD.Print("Event detected");
        if (_internalDeck.GetDiscardCount() == 0)
        {
            Card c = (Card)_discard.GetNodeOrNull("Top");
            if (c != null)
            {
                _discard.RemoveChild(c);
                c.QueueFree();
            }
        }
    }

    private void SyncDeck()
    {
        _internalDeck = MasterDeck.PlayerDeck;

    }

    public override void StartFight()
    {
        base.StartFight();
        SyncDeck();
        _internalDeck.OnDiscardChange += OnDiscardChange;

        _currentHealth = MasterScene.GetInstance().LoadPlayerHP();
        if (_currentHealth == 0) _currentHealth = MaxHealth;
        GD.Print(_currentHealth);

        _internalDeck.ForceFullReshuffle();        
        _hand.DrawOpeningHand(_internalDeck);
    }

    public override void BeginTurn()
    {
        GD.Print("Begin turn");
        base.BeginTurn();

        if (!_firstTurn)
            DrawCards(2); // _hand.AddCards(_internalDeck.Draw(2));
        _firstTurn = false;
        _actionPointLabel.Text = _actionPoints.ToString();
        _movementPointLabel.Text = _movementPoints.ToString();
    }

    void EndTargeting()
    {
        _currentlyTargeting = null;
        _hand.Unfreeze();
        _targetLabel.Visible = false;
        _state = PlayerState.SELECTING_CARD;
        _targeted = new();
    }

    public override void TakeDamage(Data.DamageType type, int amount, double critModifier, bool isCrit, bool autoResist)
    {
        base.TakeDamage(type, amount, critModifier, isCrit, autoResist);
        _sprite.Play("hurt");
    }

    public override void _PhysicsProcess(double delta)
    {
        Label deck = (Label)(_deck.GetNode("Cards"));
        deck.Text = _internalDeck.GetCardCount().ToString();
        Label discard = (Label)(_discard.GetNode("Cards"));
        discard.Text = _internalDeck.GetDiscardCount().ToString();

        if (!_isTurn)
            return;

        if (Input.IsActionJustPressed("Select"))
        {
            switch (_state)
            {
                case PlayerState.RETURNING_CARDS:
                case PlayerState.DISCARDING_CARDS:
                    _currentlyTargeting = _hand.GetSelectedCard();

                    if (_currentlyTargeting != null)
                    {
                        if (_state == PlayerState.DISCARDING_CARDS)
                        {
                            _internalDeck.Discard(_hand.RemoveCard(_currentlyTargeting, true, _discard));
                            _targetLabel.Text = "[center][color=#BB5545]Discard " + (_toDiscard - 1) + " cards[/color]";
                            _dropSound.Play();
                        }
                        else if (_state == PlayerState.RETURNING_CARDS)
                        {
                            _internalDeck.AddCard(_hand.RemoveCard(_currentlyTargeting, delete:true));
                            _targetLabel.Text = "[center][color=#BB5545]Return " + (_toDiscard - 1) + " cards to deck[/color]";
                            _dropSound.Play();
                        }

                        _currentlyTargeting = null;
                        _toDiscard--;
                        if (_toDiscard <= 0)
                        {
                            _state = PlayerState.SELECTING_CARD;
                            _targetLabel.Visible = false;
                        }
                    }
                    break;
                case PlayerState.SELECTING_CARD:

                    _currentlyTargeting = _hand.GetSelectedCard();

                    if (_currentlyTargeting != null && CanPlay(_currentlyTargeting))
                    {
                        _state = PlayerState.SELECTING_TARGETS;
                        _targetLabel.Visible = true;
                        if (_currentlyTargeting.Data.Target == TargetType.SELF)
                            _targetLabel.Text = "[center][color=#BB5545]Click to Confirm[/color]";
                        else
                            _targetLabel.Text = "[center][color=#BB5545]Select Targets[/color]";
                        _hand.Freeze();
                        _currentlyTargeting.ZIndex += 99;
                        _selectSound.Play();
                    }

                    break;
                case PlayerState.SELECTING_TARGETS:
                    Enemy clicked = GetEnemyUnderMouse();
                    if (clicked != null && !_targeted.Contains(clicked))
                    {
                        _targeted.Add(clicked);
                        _selectSound.Play();
                    }

                    if (_currentlyTargeting.Data.Target == TargetType.SELF)
                    {
                        if ((GetGlobalMousePosition()/GetWindow().Size).X > 0.35)
                            break;
                    }

                    if (IsTargetingValid())
                    {
                        Vector2 position = _currentlyTargeting.GlobalPosition;
                        _hand.RemoveCard(_currentlyTargeting);
                        _currentlyTargeting.Position = position;
                        GetParent().AddChild(_currentlyTargeting);

                        PlayCard(_targeted.ToArray(), _currentlyTargeting);

                        EndTargeting();
                        _selectSound.Play();
                    }
                    break;
                case PlayerState.GAME_OVER:
                    break;
            }

            GD.Print("Deck size: " + _internalDeck.GetCardCount());
            GD.Print("Discard size: " + _internalDeck.GetDiscardCount());
        }

        if (Input.IsActionJustPressed("Deselect") && _state == PlayerState.SELECTING_TARGETS)
        {
            EndTargeting();
        }
    }

    private async void PlayCard(ICombatant[] targets, Card card)
    {
        _sprite.Play("attack");
        await card.BeginPlayAnimation(_discard);
        _combatManager.PlayCard(this, targets, card.Data);
        _internalDeck.Discard(card.Data);

        

        _actionPointLabel.Text = _actionPoints.ToString();
        _movementPointLabel.Text = _movementPoints.ToString();
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

    private bool CanPlay(Card card)
    {
        int minCardCount = 0;
        int drawable = _internalDeck.GetCardCount() + _internalDeck.GetDiscardCount();
        foreach (DrawEffect d in card.Data.DrawEffects.Keys)
        {
            if (d == DrawEffect.RETURN || d == DrawEffect.DISCARD)
                minCardCount += card.Data.DrawEffects[d];
            else if (d == DrawEffect.DRAW)
            {
                if (drawable >= card.Data.DrawEffects[d])
                {
                    minCardCount -= card.Data.DrawEffects[d];
                    drawable -= card.Data.DrawEffects[d];
                }
                else
                {
                    minCardCount -= drawable;
                    drawable = 0;
                }
            }
        }
        return (card.Data.ActionPointCost <= _actionPoints) && (card.Data.MovementPointCost <= _movementPoints) && (_hand.GetCount() - 1 >= minCardCount);
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
                EndTargeting();
                this.EndTurn();
            }
        }
    }

    public void OnXButtonPressed()
    {
        EndTargeting();
    }

    public override void DrawCards(int n)
    {
        _drawSound.Play();
        CardData[] cards = _internalDeck.Draw(n);
        _hand.AddCards(cards);
    }

    public override void DiscardCards(int n)
    {
        _state = PlayerState.DISCARDING_CARDS;
        _toDiscard = n;
        _targetLabel.Visible = true;
        _targetLabel.Text = "[center][color=#BB5545]Discard " + n + " cards[/color]";
    }

    public override void ReturnCards(int n)
    {
        _state = PlayerState.RETURNING_CARDS;
        GD.Print(_state);
        _toDiscard = n;
        _targetLabel.Visible = true;
        _targetLabel.Text = "[center][color=#BB5545]Return " + n + " cards to deck[/color]";
    }

    public override void EndFight(EndState result)
    {
        base.EndFight(result);
        GD.Print("Fight Over - You emerge in " + result);
        _state = PlayerState.GAME_OVER;
        ((CombatMain)GetParent()).EndFight(result);
    }

}

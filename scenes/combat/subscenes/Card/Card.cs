 /*
 @author Alexander Venezia (Blunderguy)
*/

namespace Combat;

using Data;
using Godot;
using Godot.NativeInterop;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

public partial class Card : Area2D
{
	[Export] private float _maxOffset = 75f;

	private Hand _handOwner;

	private float _offset;
	public float Offset => _offset;

	private Tween _tween;

	private bool _isMoused;

	public float AngleOffset;
	public Tween AngleTween;
	private int _currentAngleGoal;

	private float _rotationFactor = 0;
	public Tween _rotationFactorTween;

	private CardData _data;
	public CardData Data => _data;

	
	public void UpdateData(CardData data)
	{
		_data = data;
		((Sprite2D)GetNode("Art")).Texture = data.Artwork;
		((RichTextLabel)GetNode("NameLabel")).Text = "[center][font_size=65]" + data.Name + "[/font_size][/center]";
		((RichTextLabel)GetNode("DescriptionLabel")).Text = "[center][font_size=35]" + data.Description + "[/font_size][/center]";

		string diceText = "";

		foreach (DamageType dt in data.Damage.Keys)
		{
			diceText += data.Damage[dt].ToString() + " " + dt + "\n";
		}
		foreach (DrawEffect de in data.DrawEffects.Keys)
		{
			diceText += de + " " + data.DrawEffects[de] + "\n";
		}
		foreach (Buff b in Data.Buffs)
		{
			string type = (b.Type == BuffType.RESISTANCE) ? Enum.GetName(b.ResistanceType) : "";
			diceText += "[color=#227733]" + ((b.Type == BuffType.RESISTANCE) ? "RES" : b.Type) + " " + type + ": " + b.Value + "[/color]\n";
		}
		foreach (Debuff d in Data.Debuffs)
		{
			diceText += "[color=#772233]" + d.Type + ": " + d.Duration + "[/color]\n";
 		}

		((RichTextLabel)GetNode("DamageLabel")).Text = "[font_size=40]" + diceText + "[/font_size]";

		((RichTextLabel)GetNode("TargetsLabel")).Text = "[right][font_size=40]" + data.Target + "[/font_size][/right]";

		((Label)GetNode("ActionPointIcon/ActionPointNumber")).Text = data.ActionPointCost.ToString();

		if (data.ActionPointCost == 0)
			((Node2D)GetNode("ActionPointIcon")).Hide();

		if (data.MovementPointCost == 0)
			((Node2D)GetNode("MovementPointIcon")).Hide();
		
	}

	private float _playAnimationVerticalPosition;
	private Vector2 _playAnimationPosition;
	private bool _animating = false;
	public async Task BeginPlayAnimation()
	{
		((CollisionShape2D)GetNode("CollisionShape2D")).Disabled = true;
		_animating = true;
		_playAnimationPosition = Position;
		_tween = CreateTween();
		_tween.TweenProperty(this, "_playAnimationPosition", Vector2.Zero, 0.25f);		
		Tween rotTween = CreateTween();
		rotTween.TweenProperty(this, "rotation_degrees", 0, 0.25f);
		
		await Task.Delay(150);

		// Play animation/shader here
		((AnimatedSprite2D)GetNode("PlayAnimation")).Play();

		await Task.Delay(750);
		this.QueueFree();		
	}

    public override void _Process(double delta)
    {
        if (_animating)
		{
			Position = _playAnimationPosition;
		}
    }

    public void SetMouseOverStatus(bool moused)
	{		
		if (moused == _isMoused)
			return;

		_isMoused = moused;

		if (moused)
		{
			_tween = CreateTween();
			_tween.TweenProperty(this, "_offset", _maxOffset, 0.25);
			_rotationFactorTween = CreateTween();
			_rotationFactorTween.TweenProperty(this, "_rotationFactor", 1.0f, 0.25f);
		}
		else
		{
			_tween = CreateTween();
			_tween.TweenProperty(this, "_offset", 0f, 0.25);
			_rotationFactorTween = CreateTween();
			_rotationFactorTween.TweenProperty(this, "_rotationFactor", 0f, 0.25f);
		}
	}

	public void SetHandOwner(Hand hand)
	{
		_handOwner = hand;
	}

	public void StartAngleTween(int goal, float duration)
	{
		if (_currentAngleGoal == goal)
			return;

		_currentAngleGoal = goal;

		AngleTween = CreateTween();
		AngleTween.TweenProperty(this, "AngleOffset", goal, duration);
	}

	public float GetRotationFactor()
	{
		return _rotationFactor;
	}

	public bool FullyExtended()
	{
		return _rotationFactor >= 0.95f;
	}

}

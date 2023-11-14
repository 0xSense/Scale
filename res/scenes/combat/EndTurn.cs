using Godot;
using System;
using System.Threading.Tasks;
using Systems.Combat;

public partial class EndTurn : Area2D
{
    private float _rotation;
    Tween _tween;

    int enter, exit;
    private bool _reacting = true;

    public void OnMouseEntered()
    {
        if (!_reacting) return;
        _tween = CreateTween();
        _tween.TweenProperty(this, "_rotation", 45, 0.25f);
        enter++;
    }

    public void OnMouseExited()
    {
        if (!_reacting) return;
        _tween = CreateTween();
        _tween.TweenProperty(this, "_rotation", 0, 0.25f);
        exit++;
        
    }

    public async void OnInputEvent(Viewport node, InputEvent e, int shapeID)
    {
        if (CombatManager.GetInstance().ToMove != CombatManager.GetInstance().Player) return;
        if (e.IsActionPressed("Select"))
            {
                _reacting = false;
                _tween = CreateTween();
                _tween.TweenProperty(this, "_rotation", 360, 1f);
                
                await Task.Delay(1000);
                _rotation = 0;
                _reacting = true;
            }
    }

    public override void _Process(double delta)
    {
        RotationDegrees = _rotation;
    }
}

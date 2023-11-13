using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class FloatingTextFactory : Node2D
{
    [Export] Theme _theme;
    [Export] int _fontSize = 30;
    private static FloatingTextFactory _instance;
    private Queue<RichTextLabel> _waitingQueue;
    public override void _Ready()
    {
        _instance = this;
        _waitingQueue = new();
    }

    public static FloatingTextFactory GetInstance()
    {
        return _instance;
    }

    public void CreateFloatingCardText(bool isHeal, bool isCrit, bool isPoison, bool isResist, int armorAmount, int amount, Vector2 position)
    {
        string color = isHeal ? "#33FF33" : "FF3333";
        color = isPoison ? "#FFBB22" : color;
        string prefix = isCrit ? "Critical! " : "";
        if (isResist)
            prefix += "Resist! ";
        string message = String.Format("[color={0}]{1}{2}[/color]", color, prefix, amount);

        Vector2 offset = Vector2.Up * 100;

        CreateFloatingText(message, position  + offset);

        if (armorAmount > 0)
            CreateFloatingText(String.Format("[color=#888888][s]{0}[/s][/color]", armorAmount), position + offset*1.5f);
    }

    public void CreateFloatingText(string message, Vector2 position)
    {
        message = String.Format("[center][font_size={0}]{1}[/font_size][/center]", _fontSize, message);

        RichTextLabel floatingText;

        if (_waitingQueue.TryPeek(out RichTextLabel label))
        {
            Refresh(label, message, position - new Vector2(300, 75));
            floatingText = label;
            _waitingQueue.Dequeue();
        }
        else
        {
            RichTextLabel newLabel = new();
            newLabel.Theme = _theme;
            newLabel.Text = message;
            newLabel.SetSize(new Vector2(600, 100), false);
            newLabel.BbcodeEnabled = true;
            
            newLabel.Position = position - new Vector2(300, 75);
            floatingText = newLabel;
        }

        AddChild(floatingText);
        TextLifetime(floatingText);
    }

    private void Refresh(RichTextLabel label, string message, Vector2 position)
    {
        label.Text = message;
        label.Position = position;
        label.Modulate = new Color(1, 1, 1, 1);
    }

    private async void TextLifetime(RichTextLabel label)
    {
        Tween positionTween = CreateTween();
        Tween alphaTween = CreateTween();
        int wait = 2;
        positionTween.TweenProperty(label, "position:y", label.Position.Y - 200, (float)wait);
        
        //label.Modulate.A -= 0.5f;
        alphaTween.TweenProperty(label, "modulate:a", 0, (float)wait);
        await Task.Delay(wait * 1000);

        RemoveChild(label);
        _waitingQueue.Enqueue(label);
    }
}

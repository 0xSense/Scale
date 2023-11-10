using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class FloatingTextFactory : Node2D
{
    [Export] int _fontSize = 50;
    private static FloatingTextFactory _instance;
    private Queue<RichTextLabel> _waitingQueue;
    public override void _Ready()
    {
        _instance = this;
        _waitingQueue = new();
        GD.Print(_instance);
    }

    public static FloatingTextFactory GetInstance()
    {
        return _instance;
    }

    public void CreateFloatingText(string message, Vector2 position)
    {
        message = String.Format("[center][font_size={0}]{1}[/font_size][/center]", _fontSize, message);

        RichTextLabel floatingText;

        if (_waitingQueue.TryPeek(out RichTextLabel label))
        {
            Refresh(label, message, position - new Vector2(150, 75));
            floatingText = label;
            GD.Print("Reused");
            _waitingQueue.Dequeue();
        }
        else
        {
            RichTextLabel newLabel = new();
            newLabel.Text = message;
            newLabel.SetSize(new Vector2(300, 100), false);
            newLabel.BbcodeEnabled = true;
            
            newLabel.Position = position - new Vector2(150, 75);
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

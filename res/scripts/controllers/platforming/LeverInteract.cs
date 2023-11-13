namespace Overworld;

using Godot;
using System;

public partial class LeverInteract : Godot.Area2D
{
	private bool _playerInsideArea;
	private Sprite2D _on;
	private Sprite2D _off;
	private bool _state;
	
    public override void _Ready()
    {
        _off = GetNode<Sprite2D>("../Off_2");
		_on = GetNode<Sprite2D>("../On_2");
		_state = true;
		_on.Visible = true;
    }

    public void _on_body_entered(Node2D body)
	{
		_playerInsideArea = true;
	}

	public void _on_body_exited(Node2D body) 
	{
		_playerInsideArea = false;
	}

    public override void _Process(double delta)
    {
        if(_playerInsideArea)
		{	
			if (Input.IsActionJustReleased("ui_interact"))
			{
				switch(_state){
					case true:
						_off.Visible = true;
						_on.Visible = false;
						_state = false;
						break;
					case false:
						_off.Visible = false;
						_on.Visible = true;
						_state = true;
						break;
				}
			}
		}
    }
}

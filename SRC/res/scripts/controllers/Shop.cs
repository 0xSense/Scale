using Godot;
using System;

public partial class Shop : Node2D
{
	public void ButtonItem(int i)
	{
		GD.Print("Give Player Item " + i);
		GetChild<GridContainer>(1).GetChild<Panel>(i - 1).Visible = false;
	}

	public void OnClosePressed()
	{
		this.Visible = false;
	}
}

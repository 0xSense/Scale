// This class manages all card types in the game
namespace Data;

using System.Collections.Generic;
using Data;
using Godot;

public partial class MasterDeck : Node2D
{	
	[Export] private Godot.Collections.Array<CardData> _cardTypes = new();
	public static List<CardData> CardTypes = new();

	public override void _Ready()
	{        
		foreach (CardData c in _cardTypes)
		{
			CardTypes.Add(c);
			GD.Print(c.UID);
		}
		_cardTypes.Clear();
	}
}

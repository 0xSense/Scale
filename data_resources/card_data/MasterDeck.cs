/*
 @author Alexander Venezia (Blunderguy)
*/

// This class manages all card types in the game
namespace Data;

using System.Collections.Generic;
using Data;
using Godot;

public partial class MasterDeck : Node
{	
	[Export] private Godot.Collections.Array<CardData> _cardTypes = new();
	public static List<CardData> CardTypes = new();

	public delegate void OnMasterDeckLoad();
	public static event OnMasterDeckLoad OnLoad;

	public override void _Ready()
	{        
		foreach (CardData c in _cardTypes)
		{
			c.Activate();
			CardTypes.Add(c);			
			// GD.Print(c.UID);
		}
		_cardTypes.Clear();
		OnLoad?.Invoke();
	}
}

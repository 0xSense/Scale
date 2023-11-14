/*
 @author Alexander Venezia (Blunderguy)
*/

// This class manages all card types in the game
namespace Data;

using System.Collections.Generic;
using Data;
using Godot;

<<<<<<< HEAD:res/scripts/controllers/combat/MasterDeck.cs
public partial class MasterDeck : Node2D
{
=======
public partial class MasterDeck : Node
{	
>>>>>>> 116aac0a80e586ee1b45b07cd2df892007715b5b:data_resources/card_data/MasterDeck.cs
	[Export] private Godot.Collections.Array<CardData> _cardTypes = new();
	public static List<CardData> CardTypes = new();

	public delegate void OnMasterDeckLoad();
	public static event OnMasterDeckLoad OnLoad;

	private static Systems.Combat.Deck _playerDeck;
	public static Systems.Combat.Deck PlayerDeck => _playerDeck;

	public override void _Ready()
<<<<<<< HEAD:res/scripts/controllers/combat/MasterDeck.cs
	{
		foreach (CardData c in _cardTypes)
		{
			c.Activate();
			CardTypes.Add(c);
			//GD.Print(c.UID);
=======
	{        
		_playerDeck = new();

		foreach (CardData c in _cardTypes)
		{
			c.Activate();
			CardTypes.Add(c);			

			_playerDeck.AddCard(c);
>>>>>>> 116aac0a80e586ee1b45b07cd2df892007715b5b:data_resources/card_data/MasterDeck.cs
		}
		_cardTypes.Clear();

		OnLoad?.Invoke();
	}
}

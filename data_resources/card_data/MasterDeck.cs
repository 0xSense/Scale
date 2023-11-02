// This class manages all card types in the game
namespace Data;

using System.Collections.Generic;
using Data;
using Godot;

public partial class MasterDeck : Node2D
{
    [Export] public static Godot.Collections.Array<CardData> CardTypes;
}
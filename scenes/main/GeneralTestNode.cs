using Systems.Combat;
using Godot;
using System;
using Data;
using System.ComponentModel.DataAnnotations.Schema;

public partial class GeneralTestNode : Node2D
{
    public override void _Ready()
    {
        Deck deck = new Deck();
        CardData card = new();
        CardData card2 = new();

    }
}

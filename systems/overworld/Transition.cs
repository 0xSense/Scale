namespace Systems.Overworld;
using Godot;
using Overworld;

/*
 As a player, when I touch an enemy, I should transition to Combat
 
 How would you emit two entities colliding? Signal, Event
 How would you save overworlds state? Resource, Pause the scene,

I need the collision area of Player and Entity
*/
public partial class Transition : Node2D
{
    //I need to GetNode here, you idiot you can use GetNode in _ready not outside of it
    public override void _Ready()
    {

    }

    public override void _Process(double delta)
    { }

    public void EngagedInCombat()
    {
        // load scene combat scene
    }
}

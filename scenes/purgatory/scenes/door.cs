using Godot;

public partial class Door : Area2D
{
	private bool _doorEntered;
	[Export] private PackedScene _sceneTransition;

	public void _on_body_entered(Node2D body)
	{
		GD.Print("Door Enter");
		_doorEntered = true;
	}
	public void _on_body_exited(Node2D body)
	{
		GD.Print("Door Exit");
		_doorEntered = false;
	}

	public override void _Ready()
	{
		// Load Resource, 
	}

	public override void _Process(double delta)
	{
		if (_doorEntered)
		{
			if (Input.IsActionJustReleased("ui_interact"))
			{
				GetTree().ChangeSceneToPacked(_sceneTransition);
				GetTree().ChangeSceneToPacked
				GetTree().Free();

			}
		}
	}
}

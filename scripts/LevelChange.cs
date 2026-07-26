using Godot;
using System;

public partial class LevelChange : Area2D
{
	[Export(PropertyHint.File, "*.tscn")] private string newLevelPath;
	[Export] private string spawnPointName;
	[Signal] public delegate void LevelChangedEventHandler(string newLevelPath, string spawnPointName, Kmicic player, float direction);
	
	public override void _Ready()
	{
		BodyEntered += OnLevelChangeAreaEntered;
	}

	public void OnLevelChangeAreaEntered(Node2D body)
	{
		if (body is Kmicic player)
		{
			EmitSignal("LevelChanged", newLevelPath, spawnPointName, player, player.GlobalPosition.X < GlobalPosition.X ? 1.0f : -1.0f);
		}
	}
}

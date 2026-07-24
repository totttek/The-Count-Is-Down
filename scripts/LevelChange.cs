using Godot;
using System;

public partial class LevelChange : Area2D
{
	[Export] private PackedScene newLevel;
	[Export] private string spawnPointName;
	[Signal] public delegate void LevelChangedEventHandler(PackedScene newLevel, string spawnPointName, CharacterBody2D player);
	
	public override void _Ready()
	{
		BodyEntered += OnLevelChangeAreaEntered;
	}

	public void OnLevelChangeAreaEntered(Node2D body)
	{
		if (body is CharacterBody2D player)
			EmitSignal("LevelChanged", newLevel, spawnPointName, player);
	}
}

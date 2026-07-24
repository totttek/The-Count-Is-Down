using Godot;
using System;

public partial class LevelContainer : Node2D
{
	[Export] private ColorRect fadeRect;
	private Level currentLevel;

	public override void _Ready()
	{
		currentLevel = GetChild<Level>(0);
		GD.Print($"Current level: {currentLevel.Name}");
		currentLevel.GetNode<LevelChange>("LevelChange").LevelChanged += LoadLevel;
	}

	public void LoadLevel(PackedScene newLevelScene, string spawnPointName, CharacterBody2D player)
	{
		// Start Fade in
		Tween fadeTween = CreateTween();
		fadeTween.TweenProperty(fadeRect, "color:a", 1.0f, 0.25f);
		//currentLevel.QueueFree();

		//var newLevelScene = ResourceLoader.Load<PackedScene>(levelPath);
		//if (newLevelScene == null)
		//{
		//	GD.PrintErr($"Failed to load level scene at path: {levelPath}");
		//	return;
		//}

		//var newLevelInstance = newLevelScene.Instantiate();
		//AddChild(newLevelInstance);

		//if (newLevelInstance is Level level)
		//{
		//	Vector2 spawnPosition = level.GetSpawnPoint(spawnPointName);
		//	player.Position = spawnPosition;
		//	level.AddChild(player);
		//}
		//else
		//{
		//	GD.PrintErr($"The loaded scene at path: {levelPath} is not of type 'Level'.");
		//}
	}
}

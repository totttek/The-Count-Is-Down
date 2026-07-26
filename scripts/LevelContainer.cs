using Godot;
using System;
using System.Linq;

public partial class LevelContainer : Node2D
{
	[Export] private ColorRect fadeRect;
	private Level currentLevel;

	[Signal] public delegate void LevelLoadedEventHandler();
	public override void _Ready()
	{
		currentLevel = GetChild<Level>(0);
		GetLevelChangers();
	}

	public async void LoadLevel(string newLevelPath, string spawnPointName, Kmicic player, float direction)
	{
		GameMaster.Instance.PauseTimer(true);
		player.SetProcessInput(false);
		player.SetDirection(direction);
		fadeRect.MouseFilter = Control.MouseFilterEnum.Stop;

		Tween fadeTween = CreateTween();
		fadeTween.TweenProperty(fadeRect, "color:a", 1.0f, 0.25f);

		ResourceLoader.LoadThreadedRequest(newLevelPath);

		await ToSignal(fadeTween, Tween.SignalName.Finished);

		player.SetProcess(false);
		player.SetPhysicsProcess(false);
		player.SetDirection(0.0f);

		while (ResourceLoader.LoadThreadedGetStatus(newLevelPath) == ResourceLoader.ThreadLoadStatus.InProgress)
		{
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}

		PackedScene newLevelScene = (PackedScene)ResourceLoader.LoadThreadedGet(newLevelPath);

		Level newLevel = (Level)newLevelScene.Instantiate();

		if (!IsInstanceValid(newLevel))
		{
			GD.PrintErr("Level not there:", newLevelPath);
			return;
		}
		
		currentLevel.QueueFree();
		AddChild(newLevel);
		currentLevel = newLevel;
		GetLevelChangers();
		DisableLevelChangers();

		player.GlobalPosition = currentLevel.GetSpawnPoint(spawnPointName);

		Camera2D camera = player.GetNode<Camera2D>("Camera2D");
		GD.Print(camera);
		camera.LimitLeft = (int)newLevel.CameraBounds.Position.X;
		camera.LimitTop = (int)newLevel.CameraBounds.Position.Y;
		camera.LimitRight = (int)newLevel.CameraBounds.Size.X;
		camera.LimitBottom = (int)newLevel.CameraBounds.Size.Y;

		player.SetProcess(true);
		player.SetPhysicsProcess(true);

		await ToSignal(GetTree().CreateTimer(0.4f), SceneTreeTimer.SignalName.Timeout);

		await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		Tween fadeOutTween = CreateTween();
		fadeOutTween.TweenProperty(fadeRect, "color:a", 0.0f, 0.25f);

		player.SetDirection(direction);
		await ToSignal(GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);
		player.SetDirection(0.0f);
		await ToSignal(fadeOutTween, Tween.SignalName.Finished);

		EnableLevelChangers();
		fadeRect.MouseFilter = Control.MouseFilterEnum.Ignore;
		player.SetProcessInput(true);
		GameMaster.Instance.PauseTimer(false);
		EmitSignal("LevelLoaded");
	}

	private void GetLevelChangers()
	{
		foreach (LevelChange levelChange in currentLevel.GetChildren().OfType<LevelChange>())
		{
			levelChange.LevelChanged += LoadLevel;
		}
	}
	
	private void DisableLevelChangers()
	{
		foreach (LevelChange levelChange in currentLevel.GetChildren().OfType<LevelChange>())
		{
			levelChange.Monitoring = false;
		}
	}

	private void EnableLevelChangers()
	{
		foreach (LevelChange levelChange in currentLevel.GetChildren().OfType<LevelChange>())
		{
			levelChange.Monitoring = true;
		}
	}
}

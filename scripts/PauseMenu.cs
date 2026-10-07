using Godot;
using System;

public partial class PauseMenu : Control
{
	[Export(PropertyHint.File, "*.tscn")] private string gameScenePath = "res://scenes/main_menu.tscn";

	private AnimationPlayer animation;

	private bool isPaused = false;

	public override void _Ready()
	{
		animation = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_cancel") && !GameMaster.Instance.IsGameOver)
		{
			if (isPaused)
				Resume();
			else
				Pause();
		}
	}

	public void Pause()
	{
		isPaused = true;
		GameMaster.Instance.PauseGame(true);
		animation.Play("pause");
	}

	public void Resume()
	{
		isPaused = false;
		GameMaster.Instance.PauseGame(false);
		animation.PlayBackwards("pause");
	}

	public async void OnMenuPressed()
	{
		isPaused = true;
		GameMaster.Instance.PauseGame(true);
		await ToSignal(GameMaster.Instance.FadeOut(), Tween.SignalName.Finished);
		GameMaster.Instance.ResetStatus();
		await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
		GetTree().ChangeSceneToFile(gameScenePath);
		await ToSignal(GameMaster.Instance.FadeIn(), Tween.SignalName.Finished);
	}
}

using Godot;

public partial class MainMenu : Node
{
	[Export(PropertyHint.File, "*.tscn")] private string gameScenePath = "res://scenes/main_game.tscn";

	public async void OnPlayPressed()
	{
		await ToSignal(GameMaster.Instance.FadeOut(), Tween.SignalName.Finished);
		await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
		GetTree().ChangeSceneToFile(gameScenePath);
		GameMaster.Instance.StartGame();
	}

	public async void OnQuitPressed()
	{
		await ToSignal(GameMaster.Instance.FadeOut(), Tween.SignalName.Finished);
		GetTree().Quit();
	}
}

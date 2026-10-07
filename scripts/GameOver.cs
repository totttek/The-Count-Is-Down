using Godot;

public partial class GameOver : Control
{
	[Export(PropertyHint.File, "*.tscn")] private string gameScenePath = "res://scenes/main_menu.tscn";
	[Export] private AudioStream saved;
	[Export] private AudioStream down;

	private AnimatedSprite2D animation;

	public override void _Ready()
	{
		animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		Visible = false;
		MouseFilter = MouseFilterEnum.Ignore;
	}

	public void ShowMenu(RecepieResult result)
	{
		MouseFilter = MouseFilterEnum.Stop;
		Visible = true;
		if (result == RecepieResult.Antidiote)
		{
			animation.Play("count_saved");
			AudioManager.Instance.PlaySound(saved);
		}
		else
		{
			animation.Play("count_down");
			AudioManager.Instance.PlaySound(down);
		}
	}

	public void OnRetryPressed()
	{
		Visible = false;
		GameMaster.Instance.ResetGame();
	}

	public async void OnMenuPressed()
	{
		GameMaster.Instance.PauseGame(true);
		await ToSignal(GameMaster.Instance.FadeOut(), Tween.SignalName.Finished);
		GameMaster.Instance.ResetStatus();
		await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
		GetTree().ChangeSceneToFile(gameScenePath);
		await ToSignal(GameMaster.Instance.FadeIn(), Tween.SignalName.Finished);
	}
}

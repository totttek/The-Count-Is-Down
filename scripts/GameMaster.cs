using Godot;
using System;
using System.Threading;

public partial class GameMaster : Node
{
	public static GameMaster Instance { get; private set; }
	public Crafting CraftingUI { get; private set; }
	public bool IsGameOver = false;

	private LevelContainer changeLevel;
	private CanvasModulate canvasModulate;
	private Kmicic player;
	private PointLight2D playerLight;
	private ColorRect fade;
	private bool timeUP = false;
	private bool paused = false;
	private float elapsed = 0.0f;
	private GameOver gameOverMenu;

	[Export(PropertyHint.File, "*.tscn")] private string finalLevelPath;
	[Export] private string endSpawnPoint = "table";
	[Export] private float timeTillDark = 45f;
	[Export] private float timeAfterDark = 15f;
	[Export] private Color startColor = Colors.White;
	[Export] private Color endColor = new(0.1f, 0.1f, 0.2f);
	[Export] private ItemManager itemsManager;

	private float totalTimeSeconds = 60f;

	public override void _Ready()
	{
		Instance = this;

		var canvasLayer = new CanvasLayer { Layer = 10};
		AddChild(canvasLayer);

		fade = new ColorRect
		{
			Color = Colors.Black,
			MouseFilter = Control.MouseFilterEnum.Ignore,
			ZIndex = 100
		};
		fade.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		canvasLayer.AddChild(fade);

		FadeIn();
	}

	private void GetDependentNodes()
	{
		totalTimeSeconds = timeTillDark + timeAfterDark;
		changeLevel = GetTree().CurrentScene.GetNode<LevelContainer>("LevelContainer");
		canvasModulate = GetTree().CurrentScene.GetNode<CanvasModulate>("CanvasModulate");
		player = GetTree().CurrentScene.GetNode<Kmicic>("Kmicic");
		playerLight = player.GetNode<PointLight2D>("Light");
		playerLight.Energy = 0;
	}

	public override void _Process(double delta)
	{
		CountDownTime(delta);
	}

	private void CountDownTime(double delta)
	{
		if (canvasModulate == null || paused || timeUP) return;

		elapsed += (float)delta;

		if (elapsed <= timeTillDark)
		{
			float t = Mathf.Clamp(elapsed/timeTillDark, 0f, 1f);
			canvasModulate.Color = startColor.Lerp(endColor, t);
			playerLight.Energy = t;
		}

		if (elapsed >= totalTimeSeconds)
		{
			timeUP = true;
			OnTimeUp();
		}
	}

	private void OnTimeUp()
	{
		player.SetProcess(false);
		player.SetPhysicsProcess(false);
		player.SetProcessInput(false);
		if(itemsManager.Items.Count < 3)
		{
			CraftingEnded("nothing", RecepieResult.Noting);
		}
		else
		{
			CraftingUI.ProcessMode = ProcessModeEnum.Always;
			CraftingUI.OpenCrafting();
		}
	}

	public async void CraftingEnded(string animationName, RecepieResult result)
	{
		IsGameOver = true;
		changeLevel.LoadLevel(finalLevelPath, endSpawnPoint, player, 0);

		await ToSignal(changeLevel, "IsScreenBlack");

		CraftingUI.Visible = false;
		player.Animation.FlipH = false;
		player.Animation.Play(animationName);
		player.Animation.Pause();

		await ToSignal(changeLevel, "LevelLoaded");
		player.SetProcessInput(false); 
		player.isDeathScene = true;

		player.Animation.Play(animationName);

		await ToSignal(player.Animation, AnimatedSprite2D.SignalName.AnimationFinished);

		await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);

		gameOverMenu.ShowMenu(result);
	}

	public async void ResetGame()
	{
		await ToSignal(FadeOut(), Tween.SignalName.Finished);

		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();

		ResetStatus();

		StartGame();
	}

	public void ResetStatus()
	{
		itemsManager.ResetInventory();
		elapsed = 0f;
		timeUP = false;
		IsGameOver = false;
		PauseTimer(true);
		changeLevel = null;
		canvasModulate = null;
		player = null;
		playerLight = null;
	}

	public async void StartGame()
	{
		while (GetTree().CurrentScene == null)
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

		GetDependentNodes();

		await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);

		await ToSignal(FadeIn(), Tween.SignalName.Finished);

		PauseTimer(false);
	}

	public void RegisterCrafting(Crafting crafting)
	{
		CraftingUI = crafting;
	}

	public void RegisterGameOver(GameOver gameOver)
	{
		gameOverMenu = gameOver;
	}

	public void PauseTimer(bool status)
	{
		paused = status;
	}

	public Tween FadeOut()
	{
		Tween fadeTween = CreateTween();
		fadeTween.TweenProperty(fade, "color:a", 1f, 0.25f);

		return fadeTween;
	}

	public Tween FadeIn()
	{
		Tween fadeTween = CreateTween();
		fadeTween.TweenProperty(fade, "color:a", 0.0f, 0.25f);

		return fadeTween;
	}

	public void PauseGame(bool status)
	{
		player.SetProcessInput(!status);
		player.SetProcess(!status);
		player.SetPhysicsProcess(!status);
		PauseTimer(status);
	}
}

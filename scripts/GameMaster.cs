using Godot;
using System;
using System.Threading;

public partial class GameMaster : Node
{
	public static GameMaster Instance { get; private set; }

	public Crafting CraftingUI { get; private set; }

	private LevelContainer changeLevel;
	private CanvasModulate canvasModulate;
	private Kmicic player;
	private PointLight2D playerLight;
	private bool timeUP = false;
	private bool paused = false;
	private float elapsed = 0.0f;

	[Export(PropertyHint.File, "*.tscn")] private string finalLevelPath;
	[Export] private string endSpawnPoint = "table";
	[Export] private float timeTillDark = 45f;
	[Export] private float timeAfterDark = 15f;
	[Export] private Color startColor = Colors.White;
	[Export] private Color endColor = new(0.1f, 0.1f, 0.2f);
	[Export] private ItemManager itemsColected;

	private float totalTimeSeconds = 60f;

	public override void _Ready()
	{
		Instance = this;
		GameStart();
	}

	public void GameStart()
	{
		totalTimeSeconds = timeTillDark + timeAfterDark;
		changeLevel = GetTree().CurrentScene.GetNode<LevelContainer>("LevelContainer");
		canvasModulate = GetTree().CurrentScene.GetNode<CanvasModulate>("CanvasModulate");
		player = GetTree().CurrentScene.GetNode<Kmicic>("Kmicic");
		playerLight = player.GetNode<PointLight2D>("Light");
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
		if(itemsColected.Items.Count < 3)
		{
			CraftingEnded();
		}
		else
		{
			CraftingUI.ProcessMode = ProcessModeEnum.Always;
			CraftingUI.OpenCrafting();
		}
	}

	public async void CraftingEnded(/*string animationName, RecepieResult result*/)
	{
		changeLevel.LoadLevel(finalLevelPath, endSpawnPoint, player, 0);
		await ToSignal(changeLevel, "LevelLoaded");
		player.SetProcessInput(false); 
	}

	public void RegisterCrafting(Crafting crafting)
	{
		CraftingUI = crafting;
	}

	public void PauseTimer(bool status)
	{
		paused = status;
	}
}

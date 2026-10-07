using Godot;
using System;

public partial class MainGame : Node
{
	[Export] private Crafting crafting;
	[Export] private GameOver gameOver;

	public override void _Ready()
	{
		GameMaster.Instance.RegisterCrafting(crafting);
		GameMaster.Instance.RegisterGameOver(gameOver);
	}

	public void OnResetPressed()
	{
		
	}
}

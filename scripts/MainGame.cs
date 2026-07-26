using Godot;
using System;

public partial class MainGame : Node
{
	[Export] private Crafting crafting;

	public override void _Ready()
	{
		GameMaster.Instance.RegisterCrafting(crafting);
	}
}

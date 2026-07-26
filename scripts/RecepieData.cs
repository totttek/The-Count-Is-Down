using Godot;
using System;

public enum RecepieResult { Antidiote, Poison, Noting }

[GlobalClass]
public partial class RecepieData : Resource
{
	[Export] public string[] IngredientsIds = new string[3];
	[Export] public RecepieResult Result = RecepieResult.Noting;
	[Export] public string ResutlAnimationName;
}

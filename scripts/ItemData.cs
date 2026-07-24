using Godot;
using System;

[GlobalClass]
public partial class ItemData : Resource
{
	[Export] public string Id;
	[Export] public string Name;
	[Export] public string Description;
	[Export] public Texture2D Icon;
}

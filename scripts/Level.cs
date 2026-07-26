using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class Level : Node2D
{
	[Export] public Rect2 CameraBounds;

	private readonly Dictionary<string, Vector2> spawnPoints = [];

	public override void _Ready()
	{
		foreach (Marker2D spawnPoint in GetChildren().OfType<Marker2D>())
		{
			spawnPoints[spawnPoint.Name] = spawnPoint.GlobalPosition;
		}
	}

	public Vector2 GetSpawnPoint(string spawnPointName)
	{
		if (spawnPoints.TryGetValue(spawnPointName, out Vector2 position))
		{
			return position;
		}
		else
		{
			GD.PrintErr($"Spawn point '{spawnPointName}' not found in level '{Name}'.");
			return Vector2.Zero;
		}
	}
}

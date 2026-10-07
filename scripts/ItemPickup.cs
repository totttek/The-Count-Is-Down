using Godot;
using System;

public partial class ItemPickup : Node2D
{
	[Export] public string itemPickupId { get; private set; }

	[Export] private ItemManager itemManager;
	[Export] private ItemData item;
	[Export] private float maxBrightness = 1.5f;

	private bool isPlayerInRange = false;

	private Sprite2D itemTexture;
	private Node2D playerInRange;

	private float brightness = 1f;

	public override void _Ready()
	{
		if (itemManager == null)
			GD.PrintErr("ItemManager is not assigned in the inspector.");

		if (item == null)
			GD.PrintErr("Item is not assigned in the inspector.");

		if (string.IsNullOrEmpty(itemPickupId))
			GD.PrintErr("itemPickupId is not assigned in the inspector.");

		if (itemManager.ItemPickupIds.Contains(itemPickupId))
			QueueFree();

		itemTexture = GetNode<Sprite2D>("Sprite2D");
	}

	public override void _Process(double delta)
	{
		if (isPlayerInRange && playerInRange != null)
		{
			brightness += (brightness < maxBrightness)? (float)delta*2 : 0f;

			itemTexture.Modulate = new Color(brightness, brightness, brightness);
		}
		else
		{
			brightness -= (brightness > 1f) ? (float)delta : 0f;;
			itemTexture.Modulate = new Color(brightness, brightness, brightness);
		}
	}

	public void OnPlayerEnteredArea(Node2D player)
	{
		isPlayerInRange = true;
		playerInRange = player;
	}

	public void OnPlayerExitedArea(Node2D player)
	{
		isPlayerInRange = false;
		if (playerInRange == player)
			playerInRange = null;
	}

	public void OnClickPickupButton(Node viewport, InputEvent @event, int shapeIdx)
	{
		if (isPlayerInRange && @event is InputEventMouseButton mouseEvent 
			&& mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			itemManager.AddItem(item);
			itemManager.MarkItemAsCollected(itemPickupId);
			GD.Print($"Picked up item: {item.Name}");
			QueueFree();
		}
	}
}

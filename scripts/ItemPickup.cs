using Godot;
using System;

public partial class ItemPickup : Node2D
{
	[Export] public string ItemPickupId { get; private set; }

	[Export] private ItemManager ItemManager;
	[Export] private ItemData Item;

	private bool isPlayerInRange = false;

	public override void _Ready()
	{
		if (ItemManager == null)
			GD.PrintErr("ItemManager is not assigned in the inspector.");

		if (Item == null)
			GD.PrintErr("Item is not assigned in the inspector.");

		if (string.IsNullOrEmpty(ItemPickupId))
			GD.PrintErr("ItemPickupId is not assigned in the inspector.");

		if (ItemManager.ItemPickupIds.Contains(ItemPickupId))
			QueueFree();
	}

	public void OnPlayerEnteredArea(Node2D player)
	{
		isPlayerInRange = true;
	}

	public void OnPlayerExitedArea(Node2D player)
	{
		isPlayerInRange = false;
	}

	public void OnClickPickupButton(Node viewport, InputEvent @event, int shapeIdx)
	{
		if (isPlayerInRange && @event is InputEventMouseButton mouseEvent 
			&& mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			ItemManager.AddItem(Item);
			ItemManager.MarkItemAsCollected(ItemPickupId);
			GD.Print($"Picked up item: {Item.Name}");
			QueueFree();
		}
	}
}

using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class ItemManager : Resource
{
	public HashSet<string> ItemPickupIds = new HashSet<string>();

	public List<ItemData> Items = new List<ItemData>();

	[Signal] public delegate void ItemAddedEventHandler(ItemData item);

	public void AddItem(ItemData item)
	{
		Items.Add(item);
		EmitSignal(SignalName.ItemAdded, item);
	}

	public void RemoveItem(ItemData item)
	{
		Items.Remove(item);
	}

	public void MarkItemAsCollected(string itemId) 
	{
		ItemPickupIds.Add(itemId);
	}

	public bool IsItemCollected(string itemId)
	{
		return ItemPickupIds.Contains(itemId);
	}

	public void ResetInventory()
	{
		Items.Clear();
		ItemPickupIds.Clear();
	}
}

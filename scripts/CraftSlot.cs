using Godot;

public partial class CraftSlot : TextureRect
{
	public ItemData PlacedItem = null;
	public InventorySlot PlacedLabel;

	[Signal] public delegate void SlotFilledEventHandler();

	public override bool _CanDropData(Vector2 atPosition, Variant data)
	{
		return data.Obj as InventorySlot != null && PlacedItem == null;
	}

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		PlacedLabel = (InventorySlot)data.Obj;
		PlacedItem = PlacedLabel.Item;
		Texture = PlacedItem.Icon;
		EmitSignal("SlotFilled");
		GD.Print($"Dropped: {data.AsString()} into slot at {GlobalPosition}");
	}
}

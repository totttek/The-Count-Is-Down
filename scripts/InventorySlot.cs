using Godot;

public partial class InventorySlot : TextureRect
{
	public ItemData Item = null;
	public bool IsPlaced = false;

	private InventorySlot Slot = null;
	private TextureRect activePreview = null;


	//public override void _Ready()
	//{
	//	if (Item != null)
	//	{
	//		Texture = Item.Icon;
	//	}
	//}

	public override Variant _GetDragData(Vector2 atPosition)
	{
		if (IsPlaced) return default;

		activePreview = new TextureRect 
		{
			Texture = Texture,
			ExpandMode = ExpandModeEnum.IgnoreSize,
			StretchMode = StretchModeEnum.KeepAspectCentered,
			Size = new Vector2(128, 128)
		};
		SetDragPreview(activePreview);
		Visible = false;
		return this;
	}

	public override void _Process(double delta)
	{
		if (activePreview != null)
			activePreview.GlobalPosition = GetGlobalMousePosition() - new Vector2(24, 24);
	}

	public override void _Notification(int what)
	{
		if (what == NotificationDragEnd)
		{
			if (activePreview == null) return;

			if (GetViewport().GuiIsDragSuccessful())
				QueueFree();
			else
				Visible = true;

			activePreview = null;
		}
	}
}

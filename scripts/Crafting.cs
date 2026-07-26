using Godot;
using System.Linq;

public partial class Crafting : Control
{
	[Export] private ItemManager inventory;
	[Export] private Control inventoryArea;
	[Export] private RecepieData[] recepies = [];

	[Export] private ColorRect modulate;

	private CraftSlot[] craftSlots;

	public override void _Ready()
	{
		
	}

	public async void OpenCrafting()
	{
		craftSlots = GetChildren().OfType<CraftSlot>().ToArray();
		foreach(CraftSlot slot in craftSlots)
			slot.SlotFilled += OnSlotFilled;

		PopulateInventory();

		modulate.MouseFilter = MouseFilterEnum.Stop;
		Tween fadeInTween = CreateTween();
		fadeInTween.TweenProperty(modulate, "color:a", 1.0f, 0.25f);

		await ToSignal(fadeInTween, Tween.SignalName.Finished);
		await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);

		Visible = true;

		Tween fadeOutTween = CreateTween();
		fadeOutTween.TweenProperty(modulate, "color:a", 0.0f, 0.25f);

		await ToSignal(fadeOutTween, Tween.SignalName.Finished);

		modulate.MouseFilter = MouseFilterEnum.Ignore;
	}

	private void PopulateInventory()
	{
		foreach (var child in inventoryArea.GetChildren())
			child.QueueFree();

		var rng = new RandomNumberGenerator();
		Vector2 areaSize = inventoryArea.Size;

		foreach (var item in inventory.Items)
		{
			var slot = new InventorySlot
			{
				Item = item,
				Texture = item.Icon,
				ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
				StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
				Size = new Vector2(96, 96)
			};

			float x = rng.RandfRange(0, areaSize.X - 64);
			float y = rng.RandfRange(0, areaSize.Y - 64);
			slot.Position = new Vector2(x, y);

			inventoryArea.AddChild(slot);
			GD.Print($"Slot size after AddChild: {slot.Size}");
		}
	}

	private RecepieData TryCraft(ItemData[] items)
	{
		foreach (var recepie in recepies)
		{
			if (Matches(recepie, items))
				return recepie;
		}

		return null;
	}

	private bool Matches(RecepieData recipe, ItemData[] selectedItems)
	{
		var remaining = selectedItems.Select(i => i.Id).ToList();

		foreach (var required in recipe.IngredientsIds)
		{
			if (required == "ANY")
				continue;

			if (!remaining.Remove(required))
				return false;
		}

		return true;
	}

	public void OnSlotFilled()
	{
		if (craftSlots.Any(slot => slot.PlacedItem == null)) return;

		var selectedItems = craftSlots.Select(s => s.PlacedItem).ToArray();
		var recepieResult = TryCraft(selectedItems);

		if (recepieResult == null)
		{
			GD.PrintErr("Recepie not found in: ", recepies);
			return;
		}

		switch (recepieResult.Result)
		{
			case RecepieResult.Antidiote:
				GD.Print("WIN!");
				break;
			case RecepieResult.Poison:
				GD.Print("Death!");
				break;
			case RecepieResult.Noting:
				GD.Print("Death anyway due to prevois poison!");
				break;
		}
		GD.Print("Crafting c:");
	}
}

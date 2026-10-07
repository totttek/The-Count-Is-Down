using Godot;

[Tool]
[GlobalClass]
public partial class AnimationOffset : AnimatedSprite2D
{
	[Export] public string[] AnimationNames = [];
	[Export] public Vector2[] Offsets = [];

	public override void _Ready()
	{
		AnimationChanged += OnAnimationChanged;
		ApplyOffset(Animation);
		ApplyRotation();
	}

	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint())
		{
			ApplyOffset(Animation);
			ApplyRotation();
		}
	}

	private void OnAnimationChanged()
	{
		ApplyOffset(Animation);
		ApplyRotation();
	}

	private void ApplyRotation()
	{
		RotationDegrees = Animation == "glaz" ? -90f : 0f;
	}

	private void ApplyOffset(string animationName)
	{
		for (int i = 0; i < AnimationNames.Length; i++)
		{
			if (AnimationNames[i] == animationName)
			{
				Position = Offsets[i];
				return;
			}
		}
		Position = Vector2.Zero;
	}
}

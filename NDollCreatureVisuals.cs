using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;
using TH_Alice.Scrpits.Dolls;

public partial class NDollCreatureVisuals : NCreatureVisuals
{
	[Export]
	public float VisualScale { get; set; } = 0.35f;

	[Export]
	public Vector2 BoundsPadding { get; set; } = Vector2.Zero;

	[Export]
	public Vector2 BoundsOffset { get; set; } = Vector2.Zero;

	[Export]
	public Vector2 VisualOffset { get; set; } = new Vector2(0, -50);

	[Export]
	public float IntentMargin { get; set; } = 50f;

	[Export]
	public Vector2 CenterOffset { get; set; } = Vector2.Zero;

	[Export]
	public bool TintWhenWax { get; set; } = true;

	[Export]
	public Color WaxTint { get; set; } = new Color(0.76f, 0.64f, 0.44f, 1f);

	public override void _Ready()
	{
		base._Ready();

		Node2D visuals = GetNode<Node2D>("%Visuals");
		if (visuals is Sprite2D sprite && sprite.Texture != null)
		{
			if (TintWhenWax && GetParent() is NCreature nCreature && nCreature.Entity?.Monster is AliceDollMonsterModel doll && doll.IsWax)
			{
				sprite.Modulate = WaxTint;
			}

			sprite.Position = VisualOffset;
			sprite.Scale = Vector2.One * VisualScale;

			Vector2 texSize = sprite.Texture.GetSize();
			Vector2 visualSize = texSize * VisualScale;
			Vector2 boundsSize = visualSize + BoundsPadding;

			Vector2 boundsCenter = sprite.Position + BoundsOffset;
			Bounds.SetAnchorsPreset(Control.LayoutPreset.TopLeft);
			Bounds.Position = boundsCenter - boundsSize / 2f;
			Bounds.Size = boundsSize;

			IntentPosition.Position = new Vector2(boundsCenter.X, boundsCenter.Y - boundsSize.Y / 2f - IntentMargin);
			VfxSpawnPosition.Position = sprite.Position + CenterOffset;
		}
	}
}

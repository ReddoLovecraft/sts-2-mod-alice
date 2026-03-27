using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;
using MegaCrit.Sts2.Core.Random;
using System;
public partial class AliceShop :  NMerchantCharacter
{
	public override void _Ready()
	{
		PlayAnimation("Idle", loop: true);
	}

	public void PlayAnimation(string anim, bool loop = false)
	{
		MegaTrackEntry megaTrackEntry = new MegaSprite(GetChild(0)).GetAnimationState().SetAnimation(anim, loop);
		if (loop)
		{
			megaTrackEntry?.SetTrackTime(megaTrackEntry.GetAnimationEnd() * Rng.Chaotic.NextFloat());
		}
	}
}

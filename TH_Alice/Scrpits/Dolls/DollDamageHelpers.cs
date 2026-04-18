using MegaCrit.Sts2.Core.Entities.Creatures;

namespace TH_Alice.Scrpits.Dolls;

public static class DollDamageHelpers
{
	[System.ThreadStatic]
	public static int CreatureCmdDamageDepth;

	public static bool IsInCreatureCmdDamage => CreatureCmdDamageDepth > 0;

	public static Creature GetBlockCreature(Creature originalTarget)
	{
		if (originalTarget.Monster is AliceDollMonsterModel)
		{
			return originalTarget;
		}
		return originalTarget.PetOwner?.Creature ?? originalTarget;
	}
}

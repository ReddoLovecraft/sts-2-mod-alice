using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using System.Threading.Tasks;

namespace TH_Alice.Scrpits.Dolls;

[HarmonyPatch(typeof(Creature), "AfterTurnStart")]
public static class DollBlockPersistence_AfterTurnStartPatch
{
	public static bool Prefix(Creature __instance, int roundNumber, CombatSide side, ref Task __result)
	{
		if (side == CombatSide.Enemy && __instance.Monster is AliceDollMonsterModel)
		{
			__result = Task.CompletedTask;
			return false;
		}
		return true;
	}
}


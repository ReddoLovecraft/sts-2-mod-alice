using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Dolls;

[HarmonyPatch(typeof(NMouseCardPlay), "IsCardInPlayZone")]
public static class DollTargetPlayZonePatch
{
	public static bool Prefix(NMouseCardPlay __instance, ref bool __result)
	{
		if (__instance.Holder?.CardModel is AliceCardModel aliceCard && aliceCard.IsTargetDoll)
		{
			var target = AccessTools.Field(typeof(NMouseCardPlay), "_target").GetValue(__instance);
			if (target != null)
			{
				__result = true;
				return false;
			}
		}
		return true;
	}
}


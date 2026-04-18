using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using System;
using System.Collections.Generic;
using System.Reflection;
using MegaCrit.Sts2.Core.Entities.Players;

namespace TH_Alice.Scrpits.Dolls;

[HarmonyPatch(typeof(CreatureCmd))]
public static class DollBlockForPetsPatch
{
	private static IEnumerable<MethodBase> TargetMethods()
	{
		foreach (MethodInfo m in AccessTools.GetDeclaredMethods(typeof(CreatureCmd)))
		{
			if (m.Name == nameof(CreatureCmd.Damage))
			{
				yield return m;
			}
		}
	}

	public static void Prefix()
	{
		DollDamageHelpers.CreatureCmdDamageDepth++;
	}

	public static void Finalizer(Exception __exception)
	{
		DollDamageHelpers.CreatureCmdDamageDepth = Math.Max(0, DollDamageHelpers.CreatureCmdDamageDepth - 1);
	}
}

[HarmonyPatch(typeof(Creature), "get_PetOwner")]
public static class DollPetOwnerForBlockPatch
{
	public static bool Prefix(Creature __instance, ref Player? __result)
	{
		if (!DollDamageHelpers.IsInCreatureCmdDamage)
		{
			return true;
		}
		if (__instance.Monster is not AliceDollMonsterModel)
		{
			return true;
		}
		__result = null;
		return false;
	}
}

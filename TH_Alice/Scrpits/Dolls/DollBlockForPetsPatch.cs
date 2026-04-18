using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;

namespace TH_Alice.Scrpits.Dolls;

[HarmonyPatch(typeof(CreatureCmd))]
public static class DollBlockForPetsPatch
{
	[HarmonyPatch(nameof(CreatureCmd.Damage), [typeof(PlayerChoiceContext), typeof(IEnumerable<Creature>), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel)])]
	[HarmonyPrefix]
	public static void DamagePrefix()
	{
		DollDamageHelpers.SuppressDollPetOwnerDuringDamage = true;
	}

	[HarmonyPatch(nameof(CreatureCmd.Damage), [typeof(PlayerChoiceContext), typeof(IEnumerable<Creature>), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel)])]
	[HarmonyFinalizer]
	public static void DamageFinalizer(Exception __exception)
	{
		DollDamageHelpers.SuppressDollPetOwnerDuringDamage = false;
	}
}

[HarmonyPatch(typeof(Creature), "get_PetOwner")]
public static class DollPetOwnerGetterPatch
{
	public static bool Prefix(Creature __instance, ref Player? __result)
	{
		if (DollDamageHelpers.SuppressDollPetOwnerDuringDamage && __instance.Monster is AliceDollMonsterModel)
		{
			__result = null;
			return false;
		}
		return true;
	}
}

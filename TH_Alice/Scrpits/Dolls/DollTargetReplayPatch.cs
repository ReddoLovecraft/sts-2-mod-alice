using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Threading.Tasks;
using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Dolls;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
public static class DollTargetReplayPatch
{
	public static bool Prefix(CardModel __instance, PlayerChoiceContext choiceContext, ref Creature? target, bool isAutoPlay, ResourceInfo resources, bool skipCardPileVisuals, ref Task __result)
	{
		if (__instance is not AliceCardModel aliceCard || !aliceCard.IsTargetDoll)
		{
			return true;
		}

		if (target != null && target.IsAlive && target.Monster is AliceDollMonsterModel)
		{
			aliceCard.LastDollTarget = target;
			return true;
		}

		Creature? last = aliceCard.LastDollTarget;
		if (last != null && last.IsAlive && last.Monster is AliceDollMonsterModel)
		{
			target = last;
			return true;
		}

		__result = Task.CompletedTask;
		return false;
	}
}

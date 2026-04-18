using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Dolls;

public static class DollCardTargetingState
{
	[ThreadStatic]
	public static bool IsTargetingDoll;

	public static bool IsAliveDollOfOwner(Creature creature, Player owner)
	{
		return creature.IsAlive && creature.Monster is AliceDollMonsterModel && creature.PetOwner == owner;
	}

	public static bool HasAliveDoll(Player owner)
	{
		return owner?.Creature?.Pets.Any(p => p.IsAlive && p.Monster is AliceDollMonsterModel) == true;
	}
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.IsValidTarget))]
public static class DollCard_IsValidTargetPatch
{
	public static bool Prefix(CardModel __instance, Creature? target, ref bool __result)
	{
		if (__instance is AliceCardModel aliceCard && aliceCard.IsTargetDoll)
		{
			if (target == null || __instance.Owner == null)
			{
				__result = false;
				return false;
			}
			__result = DollCardTargetingState.IsAliveDollOfOwner(target, __instance.Owner);
			return false;
		}
		return true;
	}
}

[HarmonyPatch]
public static class DollCard_CanPlayPatch
{
    private static MethodBase TargetMethod()
    {
        return AccessTools.Method(
            typeof(CardModel),
            nameof(CardModel.CanPlay),
            [typeof(UnplayableReason).MakeByRefType(), typeof(AbstractModel).MakeByRefType()]
        );
    }

	public static bool Prefix(CardModel __instance, ref UnplayableReason reason, ref AbstractModel? preventer, ref bool __result)
	{
		if (__instance is AliceCardModel aliceCard && aliceCard.IsTargetDoll)
		{
			Player? owner = __instance.Owner;
			if (owner == null || !DollCardTargetingState.HasAliveDoll(owner))
			{
				preventer = null;
				reason = UnplayableReason.BlockedByCardLogic;
				__result = false;
				return false;
			}
		}
		return true;
	}
}

[HarmonyPatch]
public static class DollCard_NoLivingAlliesBypassPatch
{
	private static MethodBase TargetMethod()
	{
		return AccessTools.Method(
			typeof(CardModel),
			nameof(CardModel.CanPlay),
			[typeof(UnplayableReason).MakeByRefType(), typeof(AbstractModel).MakeByRefType()]
		);
	}

	public static void Postfix(CardModel __instance, ref UnplayableReason reason, ref bool __result)
	{
		if (__instance is AliceCardModel aliceCard && aliceCard.IsTargetDoll)
		{
			Player? owner = __instance.Owner;
			if (owner != null && DollCardTargetingState.HasAliveDoll(owner))
			{
				reason &= ~UnplayableReason.NoLivingAllies;
				__result = reason == UnplayableReason.None;
			}
		}
	}
}

[HarmonyPatch(typeof(NTargetManager), "AllowedToTargetCreature")]
public static class DollCard_AllowedToTargetCreaturePatch
{
	public static bool Prefix(NTargetManager __instance, Creature creature, ref bool __result)
	{
		if (DollCardTargetingState.IsTargetingDoll)
		{
			__result = creature.IsAlive && creature.Monster is AliceDollMonsterModel && LocalContext.IsMe(creature.PetOwner);
			return false;
		}
		return true;
	}
}

[HarmonyPatch(typeof(NMouseCardPlay), "TargetSelection")]
public static class DollCard_MouseTargetSelectionPatch
{
	public static bool Prefix(NMouseCardPlay __instance, ref Task __result, TargetMode targetMode)
	{
		CardModel? card = AccessTools.Property(typeof(NCardPlay), "Card").GetValue(__instance) as CardModel;
		NCard? cardNode = AccessTools.Property(typeof(NCardPlay), "CardNode").GetValue(__instance) as NCard;
		if (cardNode == null || card == null)
		{
			return true;
		}
		if (card is AliceCardModel aliceCard && aliceCard.IsTargetDoll)
		{
			__result = TargetDollAsync(__instance, targetMode, cardNode, card);
			return false;
		}
		return true;
	}

	private static async Task TargetDollAsync(NMouseCardPlay cardPlay, TargetMode targetMode, NCard cardNode, CardModel card)
	{
		AccessTools.Method(typeof(NCardPlay), "TryShowEvokingOrbs").Invoke(cardPlay, null);
		cardNode.CardHighlight.AnimFlash();
		AccessTools.Method(typeof(NCardPlay), "CenterCard").Invoke(cardPlay, null);

		NTargetManager instance = NTargetManager.Instance;
		Callable onCreatureHover = (Callable)AccessTools.Field(typeof(NMouseCardPlay), "_onCreatureHoverCallable").GetValue(cardPlay);
		Callable onCreatureUnhover = (Callable)AccessTools.Field(typeof(NMouseCardPlay), "_onCreatureUnhoverCallable").GetValue(cardPlay);
		AccessTools.Field(typeof(NMouseCardPlay), "_signalsConnected").SetValue(cardPlay, true);
		instance.Connect(NTargetManager.SignalName.CreatureHovered, onCreatureHover);
		instance.Connect(NTargetManager.SignalName.CreatureUnhovered, onCreatureUnhover);

		DollCardTargetingState.IsTargetingDoll = true;
		try
		{
			Func<bool> shouldFinish = (Func<bool>)AccessTools.Method(typeof(NMouseCardPlay), "IsCardInCancelZone").CreateDelegate(typeof(Func<bool>), cardPlay);
			CancellationTokenSource? cts = AccessTools.Field(typeof(NMouseCardPlay), "_cancellationTokenSource").GetValue(cardPlay) as CancellationTokenSource;
			Func<bool> exitEarly = () => shouldFinish() || (cts?.IsCancellationRequested ?? false);

			instance.StartTargeting(TargetType.AnyAlly, cardNode, targetMode, exitEarly, node =>
			{
				if (node is NCreature nCreature)
				{
					return DollCardTargetingState.IsAliveDollOfOwner(nCreature.Entity, card.Owner);
				}
				return false;
			});

			Node node = await instance.SelectionFinished();
			if (node is NCreature nTarget)
			{
				AccessTools.Field(typeof(NMouseCardPlay), "_target").SetValue(cardPlay, nTarget.Entity);
			}
		}
		finally
		{
			DollCardTargetingState.IsTargetingDoll = false;
			AccessTools.Method(typeof(NMouseCardPlay), "DisconnectTargetingSignals").Invoke(cardPlay, null);
		}
	}
}

[HarmonyPatch(typeof(NControllerCardPlay), nameof(NControllerCardPlay.Start))]
public static class DollCard_ControllerStartPatch
{
	public static bool Prefix(NControllerCardPlay __instance)
	{
		CardModel? card = __instance.Holder?.CardModel;
		if (card is AliceCardModel aliceCard && aliceCard.IsTargetDoll)
		{
			TaskHelper.RunSafely(StartDollTargetingAsync(__instance));
			return false;
		}
		return true;
	}

	private static async Task StartDollTargetingAsync(NControllerCardPlay cardPlay)
	{
		CardModel? card = AccessTools.Property(typeof(NCardPlay), "Card").GetValue(cardPlay) as CardModel;
		NCard? cardNode = AccessTools.Property(typeof(NCardPlay), "CardNode").GetValue(cardPlay) as NCard;
		if (card == null || cardNode == null)
		{
			return;
		}
		if (!card.CanPlay(out UnplayableReason _, out AbstractModel _))
		{
			AccessTools.Method(typeof(NCardPlay), "CancelPlayCard").Invoke(cardPlay, null);
			return;
		}
		AccessTools.Method(typeof(NCardPlay), "TryShowEvokingOrbs").Invoke(cardPlay, null);
		cardNode.CardHighlight.AnimFlash();
		AccessTools.Method(typeof(NCardPlay), "CenterCard").Invoke(cardPlay, null);
		await DollCard_ControllerSingleTargetPatch.TargetDollAsync(cardPlay);
	}
}

[HarmonyPatch(typeof(NControllerCardPlay), "SingleCreatureTargeting")]
public static class DollCard_ControllerSingleTargetPatch
{
	public static bool Prefix(NControllerCardPlay __instance, ref Task __result, TargetType targetType)
	{
		CardModel? card = AccessTools.Property(typeof(NCardPlay), "Card").GetValue(__instance) as CardModel;
		if (card is AliceCardModel aliceCard && aliceCard.IsTargetDoll)
		{
			__result = TargetDollAsync(__instance);
			return false;
		}
		return true;
	}

	internal static async Task TargetDollAsync(NControllerCardPlay cardPlay)
	{
		CardModel? card = AccessTools.Property(typeof(NCardPlay), "Card").GetValue(cardPlay) as CardModel;
		if (card == null)
		{
			return;
		}
		NTargetManager targetManager = NTargetManager.Instance;
		targetManager.Connect(NTargetManager.SignalName.CreatureHovered, Callable.From<NCreature>(n => AccessTools.Method(typeof(NCardPlay), "OnCreatureHover").Invoke(cardPlay, new object[] { n })));
		targetManager.Connect(NTargetManager.SignalName.CreatureUnhovered, Callable.From<NCreature>(n => AccessTools.Method(typeof(NCardPlay), "OnCreatureUnhover").Invoke(cardPlay, new object[] { n })));

		DollCardTargetingState.IsTargetingDoll = true;
		try
		{
			targetManager.StartTargeting(TargetType.AnyAlly, AccessTools.Property(typeof(NCardPlay), "CardNode").GetValue(cardPlay) as NCard, TargetMode.Controller, () => !GodotObject.IsInstanceValid(cardPlay), node =>
			{
				if (node is NCreature nCreature)
				{
					return DollCardTargetingState.IsAliveDollOfOwner(nCreature.Entity, card.Owner);
				}
				return false;
			});

			List<Creature> dolls = card.Owner.Creature.Pets.Where(p => DollCardTargetingState.IsAliveDollOfOwner(p, card.Owner)).ToList();
			List<NCreature> nodes = dolls.Select(c => NCombatRoom.Instance.GetCreatureNode(c)).OfType<NCreature>().ToList();
			if (nodes.Count == 0)
			{
				AccessTools.Method(typeof(NCardPlay), "CancelPlayCard").Invoke(cardPlay, null);
				return;
			}
			NCombatRoom.Instance.RestrictControllerNavigation(nodes.Select(n => n.Hitbox));
			nodes.First().Hitbox.TryGrabFocus();
			NCreature nTarget = (NCreature)(await targetManager.SelectionFinished());
			if (GodotObject.IsInstanceValid(cardPlay))
			{
				if (nTarget != null)
				{
					AccessTools.Method(typeof(NCardPlay), "TryPlayCard").Invoke(cardPlay, new object[] { nTarget.Entity });
				}
				else
				{
					AccessTools.Method(typeof(NCardPlay), "CancelPlayCard").Invoke(cardPlay, null);
				}
			}
		}
		finally
		{
			DollCardTargetingState.IsTargetingDoll = false;
		}
	}
}

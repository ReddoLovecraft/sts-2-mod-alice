using HarmonyLib;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.Saves;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TH_Alice.Scrpits.Dolls;

public static class DollTurnPhase
{
	public static float MoveOutSecondsFast { get; set; } = 0.12f;
	public static float MoveOutSecondsStandard { get; set; } = 0.22f;
	public static float MoveBackSecondsFast { get; set; } = 0.12f;
	public static float MoveBackSecondsStandard { get; set; } = 0.22f;
	public static float PauseBeforeActionFast { get; set; } = 0.06f;
	public static float PauseBeforeActionStandard { get; set; } = 0.12f;
	public static float PauseBetweenDollsFast { get; set; } = 0.08f;
	public static float PauseBetweenDollsStandard { get; set; } = 0.14f;

	public static float AllAttackFrontOffset { get; set; } = 80f;
	public static float SingleAttackFrontOffset { get; set; } = 40f;

	public static async Task ExecuteAll(CombatState combatState)
	{
		PlayerChoiceContext ctx = new ThrowingPlayerChoiceContext();
		foreach (Player player in combatState.Players)
		{
			Creature owner = player.Creature;
			foreach (Creature dollCreature in owner.Pets.Where(p => p.IsAlive && p.Monster is AliceDollMonsterModel).ToList())
			{
				await ExecuteSingle(combatState, dollCreature, ctx);
				await Cmd.CustomScaledWait(PauseBetweenDollsFast, PauseBetweenDollsStandard);
			}
		}
	}

	public static async Task ExecuteSingle(CombatState combatState, Creature dollCreature, PlayerChoiceContext ctx)
	{
		if (!combatState.ContainsCreature(dollCreature))
		{
			return;
		}
		NCombatRoom room = NCombatRoom.Instance;
		if (room == null)
		{
			return;
		}
		NCreature nCreature = room.GetCreatureNode(dollCreature);
		if (nCreature != null)
		{
			nCreature.ToggleIsInteractable(true);
			await nCreature.RefreshIntents();
		}
		if (dollCreature.Monster is AliceDollMonsterModel doll)
		{
			Vector2? originalPos = nCreature?.Position;
			Vector2? attackPos = GetAttackPosition(combatState, room, dollCreature, nCreature);
			if (originalPos.HasValue && attackPos.HasValue && nCreature != null)
			{
				await TweenTo(nCreature, attackPos.Value, MoveOutSecondsFast, MoveOutSecondsStandard, outEase: true);
				await Cmd.CustomScaledWait(PauseBeforeActionFast, PauseBeforeActionStandard);
			}
			await doll.PerformIntent(ctx);
			if (dollCreature.IsDead || nCreature == null || !GodotObject.IsInstanceValid(nCreature))
			{
				return;
			}
			if (originalPos.HasValue && attackPos.HasValue)
			{
				await TweenTo(nCreature, originalPos.Value, MoveBackSecondsFast, MoveBackSecondsStandard, outEase: false);
			}
			await nCreature.RefreshIntents();
		}
	}

	private static Vector2? GetAttackPosition(CombatState combatState, NCombatRoom room, Creature dollCreature, NCreature? nCreature)
	{
		if (nCreature == null || dollCreature.Monster == null)
		{
			return null;
		}
		bool hasAttack = dollCreature.Monster.NextMove.Intents.Any(i => i is AttackIntent);
		if (!hasAttack)
		{
			return null;
		}
		if (combatState.HittableEnemies.Count == 0)
		{
			return null;
		}

		bool isAll = dollCreature.Monster is NETHERLAND or BOMB or ROUNDTABLE;
		if (isAll)
		{
			Vector2 sum = Vector2.Zero;
			int count = 0;
			foreach (Creature enemy in combatState.HittableEnemies)
			{
				NCreature enemyNode = room.GetCreatureNode(enemy);
				if (enemyNode != null)
				{
					sum += enemyNode.Position;
					count++;
				}
			}
			if (count == 0)
			{
				return null;
			}
			Vector2 center = sum / count;
			return new Vector2(center.X - AllAttackFrontOffset, center.Y);
		}
		else
		{
			Creature target = combatState.RunState.Rng.CombatTargets.NextItem(combatState.HittableEnemies);
			NCreature targetNode = room.GetCreatureNode(target);
			if (targetNode == null)
			{
				return null;
			}
			Vector2 dir = (targetNode.Position - nCreature.Position).Normalized();
			if (dir == Vector2.Zero)
			{
				dir = Vector2.Right;
			}
			return targetNode.Position - dir * SingleAttackFrontOffset;
		}
	}

	private static async Task TweenTo(NCreature nCreature, Vector2 target, float fastSeconds, float standardSeconds, bool outEase)
	{
		float seconds = GetScaledSeconds(fastSeconds, standardSeconds);
		Tween tween = nCreature.CreateTween();
		tween.TweenProperty(nCreature, "position", target, seconds)
			.SetTrans(Tween.TransitionType.Sine)
			.SetEase(outEase ? Tween.EaseType.Out : Tween.EaseType.InOut);
		await nCreature.ToSignal(tween, Tween.SignalName.Finished);
	}

	private static float GetScaledSeconds(float fastSeconds, float standardSeconds)
	{
		if (NonInteractiveMode.IsActive || SaveManager.Instance.PrefsSave.FastMode == FastModeType.Instant)
		{
			return 0f;
		}
		return SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast ? fastSeconds : standardSeconds;
	}
}

[HarmonyPatch(typeof(CombatManager), "SwitchFromPlayerToEnemySide")]
public static class DollTurnPhase_SwitchFromPlayerToEnemySidePatch
{
	public static void Prefix(CombatManager __instance, ref Func<Task>? actionDuringEnemyTurn)
	{
		Func<Task>? original = actionDuringEnemyTurn;
		actionDuringEnemyTurn = async () =>
		{
			if (original != null)
			{
				await original();
			}
			CombatState? combatState = AccessTools.Field(typeof(CombatManager), "_state").GetValue(__instance) as CombatState;
			if (combatState != null && combatState.CurrentSide == CombatSide.Enemy)
			{
				await DollTurnPhase.ExecuteAll(combatState);
			}
		};
	}
}

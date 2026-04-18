using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TH_Alice.Scrpits.Cards;
using TH_Alice.Scrpits.Main;
using TH_Alice.Scrpits.Powers;
using TH_Alice.TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Dolls;

public abstract class AliceDollMonsterModel : CustomMonsterModel
{
	public abstract int BaseHp { get; }
	public abstract int MaxCount { get; }
	public virtual bool ParticipatesInDamageShare => true;

	public int Intent { get; set; }
	public bool IsWax { get; private set; }
	public int WaxTurnsRemaining { get; private set; }

	private int _lastKnownHp;
	private bool _wasRecycled;

	public override int MinInitialHp => BaseHp;
	public override int MaxInitialHp => BaseHp;
	public override bool IsHealthBarVisible => true;
	public override LocString Title => MonsterModel.L10NMonsterLookup(GetType().Name + ".name");

	public override NCreatureVisuals? CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene($"res://ArtWorks/Dolls/{GetType().Name}.tscn");

	public void ConfigureAsWax(bool isWax)
	{
		AssertMutable();
		IsWax = isWax;
		WaxTurnsRemaining = isWax ? 3 : 0;
	}

	protected string IntentLocPrefix => GetType().Name + ".moves.INTENT";
	protected string MaxIntentLocPrefix => GetType().Name + ".moves.MAXINTENT";

	public virtual Task PerformIntent(PlayerChoiceContext choiceContext, bool repeatable = true) => Task.CompletedTask;
	public virtual Task PerformMaxIntent(PlayerChoiceContext choiceContext) => Task.CompletedTask;

	protected Player? DollOwner => Creature.PetOwner;
	protected Creature? DollOwnerCreature => DollOwner?.Creature;

	public override Task AfterCreatureAddedToCombat(Creature creature)
	{
		if (creature == Creature)
		{
			_lastKnownHp = creature.CurrentHp;
		}
		return Task.CompletedTask;
	}

	public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
	{
		if (creature == Creature)
		{
			_lastKnownHp = creature.CurrentHp;
		}
		return Task.CompletedTask;
	}

	public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		await base.AfterDamageReceived(choiceContext, target, result, props, dealer, cardSource);
		if (target != Creature || Creature.IsDead)
		{
			return;
		}
		Creature? owner = DollOwnerCreature;
		if (owner != null && owner.HasPower<DollRevengePower>() && result.TotalDamage > 0)
		{
			foreach(Creature allyDoll in owner.Pets.Where(p => p.IsAlive && p.Monster is AliceDollMonsterModel))
			{
			    await PowerCmd.Apply<StrengthPower>(allyDoll, result.TotalDamage, null, null);
			}
		}
		if (owner != null && owner.HasPower<DollJudgmentPower>())
		{
			await PowerCmd.Apply<FlexPotionPower>(owner, owner.GetPowerAmount<DollJudgmentPower>(), null, null);
		}
		await Task.CompletedTask;
	}

	public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
	{
		if (creature != Creature)
		{
			return;
		}
		Player? owner = DollOwner;
		if (!_wasRecycled && owner?.GetRelic<BottlePowder>() != null)
		{
			TheBombPower? bomb = await PowerCmd.Apply<TheBombPower>(owner.Creature, 1m, null, null);
			bomb?.SetDamage(_lastKnownHp);
		}
	}

	protected async Task TriggerSilk()
	{
		Player? owner = DollOwner;
		if (owner?.GetRelic<Silk>() != null)
		{
			await PowerCmd.Apply<EnergyNextTurnPower>(owner.Creature, 1m, owner.Creature, null);
		}
	}

	protected async Task MaybeRepeatByLube(PlayerChoiceContext choiceContext, bool repeatable)
	{
		if (!repeatable)
		{
			return;
		}
		Creature? ownerCreature = DollOwnerCreature;
		if (ownerCreature != null && ownerCreature.HasPower<LubePower>())
		{
			int times = ownerCreature.GetPowerAmount<LubePower>();
			for (int i = 0; i < times; i++)
			{
				await PerformIntent(choiceContext, repeatable: false);
			}
		}
	}

	public async Task Recycle(PlayerChoiceContext choiceContext)
	{
		Player? owner = DollOwner;
		CombatState? combatState = Creature.CombatState;
		if (owner?.Creature != null && combatState != null)
		{
			int parts = ToolBox.GetRecycleNum(Creature, this);
			if (parts > 0)
			{
				List<CardModel> list = new List<CardModel>();
				for (int i = 0; i < parts; i++)
				{
					list.Add(combatState.CreateCard<DollPart>(owner));
				}
				CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Draw, addedByPlayer: true, CardPilePosition.Random));
			}
			if (owner.Creature.HasPower<GirlDollPower>())
			{
				int inc = owner.Creature.GetPowerAmount<GirlDollPower>();
				foreach (Creature allyDoll in owner.Creature.Pets.Where(p => p.IsAlive && p.Monster is AliceDollMonsterModel))
				{
					await CreatureCmd.Heal(allyDoll, inc);
				}
			}
		}
		_wasRecycled = true;
		await CreatureCmd.Kill(Creature, force: true);
	}

	public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (side == CombatSide.Player && IsWax && Creature.IsAlive)
		{
			WaxTurnsRemaining--;
			if (WaxTurnsRemaining <= 0)
			{
				await CreatureCmd.Kill(Creature, force: true);
			}
		}
	}

	protected abstract IEnumerable<AbstractIntent> GetDisplayIntents();

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		var intentState = new MoveState("INTENT", _ => Task.CompletedTask, GetDisplayIntents().ToArray());
		return new MonsterMoveStateMachine([intentState], intentState);
	}
}

public sealed class SHANGHAI : AliceDollMonsterModel
{
	public override int BaseHp => 10;
	public override int MaxCount => 10;

	public SHANGHAI()
	{
		Intent = 6;
	}

	protected override IEnumerable<AbstractIntent> GetDisplayIntents() => new AbstractIntent[]
	{
		new DollSingleAttackIntent(IntentLocPrefix, () => Intent, () => Intent)
	};

	public override async Task PerformIntent(PlayerChoiceContext choiceContext, bool repeatable = true)
	{
		CombatState? combat = Creature.CombatState;
		if (combat == null)
		{
			return;
		}
		Creature? owner = DollOwnerCreature;
		if (owner == null)
		{
			return;
		}
		if (DollOwner?.GetRelic<PowerCard>() != null)
		{
			if (combat.HittableEnemies.Count > 0)
			{
				await CreatureCmd.Damage(choiceContext, combat.HittableEnemies, Intent, ValueProp.Move, owner, null);
			}
		}
		else
		{
			Creature? target = combat.HittableEnemies.FirstOrDefault(m => m.HasPower<MgrPower>());
			target ??= combat.RunState.Rng.CombatTargets.NextItem(combat.HittableEnemies);
			if (target != null && target.IsAlive)
			{
				await CreatureCmd.Damage(choiceContext, target, Intent, ValueProp.Move, owner, null);
			}
		}
		await TriggerSilk();
		await MaybeRepeatByLube(choiceContext, repeatable);
	}

	public override Task PerformMaxIntent(PlayerChoiceContext choiceContext) => PerformIntent(choiceContext);
}

public sealed class XIZANG : AliceDollMonsterModel
{
	public override int BaseHp => 6;
	public override int MaxCount => 6;

	public XIZANG()
	{
		Intent = 0;
	}

	public override bool ParticipatesInDamageShare => true;

	protected override IEnumerable<AbstractIntent> GetDisplayIntents() => new AbstractIntent[]
	{
		new DollHiddenIntent(IntentLocPrefix, () => Intent)
	};

	public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
	{
		await base.AfterDeath(choiceContext, creature, wasRemovalPrevented, deathAnimLength);
		if (Creature.IsDead && creature != Creature)
		{
			return;
		}
		Player? owner = DollOwner;
		if (owner == null)
		{
			return;
		}
		if (creature.Monster is not AliceDollMonsterModel)
		{
			return;
		}
		if (creature.PetOwner != owner)
		{
			return;
		}
		if (creature.Monster is XIZANG && creature != Creature)
		{
			return;
		}
		if (creature != Creature)
		{
			Creature? primary = owner.Creature.Pets.FirstOrDefault(p => p.IsAlive && p.Monster is XIZANG);
			if (primary != Creature)
			{
				return;
			}
		}
		await ToolBox.MakeDoll<XiZangPower>(owner.Creature);
	}

	public override async Task PerformMaxIntent(PlayerChoiceContext choiceContext)
	{
		Creature? owner = DollOwnerCreature;
		if (owner == null)
		{
			return;
		}
		await ToolBox.MakeRandomDoll(owner);
	}
}

public sealed class PENGLAI : AliceDollMonsterModel
{
	public override int BaseHp => 4;
	public override int MaxCount => 4;

	public PENGLAI()
	{
		Intent = 4;
	}

	protected override IEnumerable<AbstractIntent> GetDisplayIntents() => new AbstractIntent[]
	{
		new DollBuffIntent(IntentLocPrefix, () => Intent)
	};

	public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (target != Creature || Creature.IsDead)
		{
			return;
		}
		if (result.UnblockedDamage <= 0)
		{
			return;
		}
		CombatState? combat = Creature.CombatState;
		Creature? owner = DollOwnerCreature;
		if (combat == null || owner == null || combat.HittableEnemies.Count == 0)
		{
			return;
		}
		await CreatureCmd.Damage(choiceContext, combat.HittableEnemies, Creature.CurrentHp, ValueProp.Move, owner, null);
	}

	public override async Task PerformIntent(PlayerChoiceContext choiceContext, bool repeatable = true)
	{
		await CreatureCmd.GainMaxHp(Creature, Intent);
		await TriggerSilk();
		await MaybeRepeatByLube(choiceContext, repeatable);
	}

	public override async Task PerformMaxIntent(PlayerChoiceContext choiceContext)
	{
		CombatState? combat = Creature.CombatState;
		Creature? owner = DollOwnerCreature;
		if (combat != null && owner != null && combat.HittableEnemies.Count > 0)
		{
			await CreatureCmd.Damage(choiceContext, combat.HittableEnemies, Creature.MaxHp, ValueProp.Move, owner, null);
		}
		await Recycle(choiceContext);
	}
}

public sealed class HINA : AliceDollMonsterModel
{
	public override int BaseHp => 5;
	public override int MaxCount => 1;

	public HINA()
	{
		Intent = 1;
	}

	protected override IEnumerable<AbstractIntent> GetDisplayIntents() => new AbstractIntent[]
	{
		new DollBuffIntent(IntentLocPrefix, () => Intent)
	};

	public override async Task PerformIntent(PlayerChoiceContext choiceContext, bool repeatable = true)
	{
		Creature? owner = DollOwnerCreature;
		if (owner == null)
		{
			return;
		}
		await PowerCmd.Apply<ArtifactPower>(owner, Intent, owner, null);
		await TriggerSilk();
		await MaybeRepeatByLube(choiceContext, repeatable);
	}

	public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
	{
		await base.AfterDeath(choiceContext, creature, wasRemovalPrevented, deathAnimLength);
		if (creature != Creature)
		{
			return;
		}
		Creature? owner = DollOwnerCreature;
		if (owner == null)
		{
			return;
		}
		List<PowerModel> debuffs = owner.Powers.Where(p => p.Type == PowerType.Debuff).ToList();
		for (int i = debuffs.Count - 1; i >= 0; i--)
		{
			await PowerCmd.Remove(debuffs[i]);
		}
	}

	public override async Task PerformMaxIntent(PlayerChoiceContext choiceContext)
	{
		Creature? owner = DollOwnerCreature;
		if (owner != null)
		{
			List<PowerModel> debuffs = owner.Powers.Where(p => p.Type == PowerType.Debuff).ToList();
			for (int i = debuffs.Count - 1; i >= 0; i--)
			{
				await PowerCmd.Remove(debuffs[i]);
			}
		}
		await Recycle(choiceContext);
	}
}

public sealed class GOLIATH : AliceDollMonsterModel
{
	public override int BaseHp => 20;
	public override int MaxCount => 2;

	public GOLIATH()
	{
		Intent = 5;
	}

	protected override IEnumerable<AbstractIntent> GetDisplayIntents() => new AbstractIntent[]
	{
		new DollDefendIntent(IntentLocPrefix, () => Intent)
	};

	public override async Task PerformIntent(PlayerChoiceContext choiceContext, bool repeatable = true)
	{
		Player? owner = DollOwner;
		if (owner == null)
		{
			return;
		}
		int dollCount = ToolBox.GetDollCount(owner.Creature);
		int block = Intent + dollCount;
		foreach (Creature dollCreature in owner.Creature.Pets.Where(p => p.IsAlive && p.Monster is AliceDollMonsterModel))
		{
			await CreatureCmd.GainBlock(dollCreature, block, 0, null);
		}
		await TriggerSilk();
		await MaybeRepeatByLube(choiceContext, repeatable);
	}

	public override async Task PerformMaxIntent(PlayerChoiceContext choiceContext)
	{
		Player? owner = DollOwner;
		if (owner == null)
		{
			return;
		}
		foreach (Creature dollCreature in owner.Creature.Pets.Where(p => p.IsAlive && p.Monster is AliceDollMonsterModel))
		{
			await PowerCmd.Apply<BlurPower>(dollCreature, 1m, owner.Creature, null);
		}
	}
}

public sealed class ROUNDTABLE : AliceDollMonsterModel
{
	public override int BaseHp => 12;
	public override int MaxCount => 12;

	public ROUNDTABLE()
	{
		Intent = 2;
	}

	protected override IEnumerable<AbstractIntent> GetDisplayIntents() => new AbstractIntent[]
	{
		new DollBuffIntent(IntentLocPrefix, () => Intent),
		new DollMultiAttackIntent(IntentLocPrefix, () => Intent, () => 1, () => Intent)
	};

	public override async Task PerformIntent(PlayerChoiceContext choiceContext, bool repeatable = true)
	{
		CombatState? combat = Creature.CombatState;
		Creature? owner = DollOwnerCreature;
		if (combat == null || owner == null)
		{
			return;
		}
		await PowerCmd.Apply<PlatingPower>(owner, Intent, owner, null);
		if (combat.HittableEnemies.Count > 0)
		{
		await CreatureCmd.Damage(choiceContext, combat.HittableEnemies, Intent, ValueProp.Move, owner, null);
		}
		await TriggerSilk();
		await MaybeRepeatByLube(choiceContext, repeatable);
	}

	public override async Task PerformMaxIntent(PlayerChoiceContext choiceContext)
	{
		Player? owner = DollOwner;
		if (owner == null)
		{
			return;
		}
		foreach (Creature dollCreature in owner.Creature.Pets.Where(p => p.IsAlive && p.Monster is AliceDollMonsterModel))
		{
			await PowerCmd.Apply<PlatingPower>(dollCreature, 1m, owner.Creature, null);
		}
	}
}

public sealed class FRANCE : AliceDollMonsterModel
{
	public override int BaseHp => 10;
	public override int MaxCount => 8;

	public FRANCE()
	{
		Intent = 4;
	}

	protected override IEnumerable<AbstractIntent> GetDisplayIntents() => new AbstractIntent[]
	{
		new DollMultiAttackIntent(IntentLocPrefix, () => Intent, () => 2, () => Intent)
	};

	public override async Task PerformIntent(PlayerChoiceContext choiceContext, bool repeatable = true)
	{
		CombatState? combat = Creature.CombatState;
		Creature? owner = DollOwnerCreature;
		if (combat == null || owner == null || combat.HittableEnemies.Count == 0)
		{
			return;
		}
		for (int i = 0; i < 2; i++)
		{
			Creature? target = combat.HittableEnemies.FirstOrDefault(m => m.HasPower<MgrPower>());
			target ??= combat.RunState.Rng.CombatTargets.NextItem(combat.HittableEnemies);
			if (target != null && target.IsAlive)
			{
				await CreatureCmd.Damage(choiceContext, target, Intent, ValueProp.Move, owner, null);
			}
		}
		await TriggerSilk();
		await MaybeRepeatByLube(choiceContext, repeatable);
	}

	public override async Task PerformMaxIntent(PlayerChoiceContext choiceContext)
	{
		CombatState? combat = Creature.CombatState;
		if (combat == null)
		{
			return;
		}
		int count = combat.HittableEnemies.Count(e => e.Monster != null && !e.Monster.IntendsToAttack);
		for (int i = 0; i < count; i++)
		{
			await PerformIntent(choiceContext);
		}
	}
}

public sealed class ORL : AliceDollMonsterModel
{
	public override int BaseHp => 10;
	public override int MaxCount => 3;

	public ORL()
	{
		Intent = 6;
	}

	protected override IEnumerable<AbstractIntent> GetDisplayIntents() => new AbstractIntent[]
	{
		new DollDefendIntent(IntentLocPrefix, () => Intent),
		new DollSingleAttackIntent(IntentLocPrefix, () => Creature.Block, () => Intent)
	};

	public override async Task PerformIntent(PlayerChoiceContext choiceContext, bool repeatable = true)
	{
		CombatState? combat = Creature.CombatState;
		Creature? owner = DollOwnerCreature;
		if (combat == null || owner == null)
		{
			return;
		}
		await CreatureCmd.GainBlock(Creature, Intent, 0, null);
		if (combat.HittableEnemies.Count > 0)
		{
			Creature target = combat.RunState.Rng.CombatTargets.NextItem(combat.HittableEnemies);
			await CreatureCmd.Damage(choiceContext, target, Creature.Block, ValueProp.Move, owner, null);
		}
		await TriggerSilk();
		await MaybeRepeatByLube(choiceContext, repeatable);
	}

	public override async Task PerformMaxIntent(PlayerChoiceContext choiceContext)
	{
		if (Creature.Block > 0)
		{
			await CreatureCmd.GainBlock(Creature, Creature.Block, 0, null);
		}
		else
		{
			await CreatureCmd.GainBlock(Creature, 12, 0, null);
		}
	}
}

public sealed class NETHERLAND : AliceDollMonsterModel
{
	public override int BaseHp => 30;
	public override int MaxCount => 4;
	public override bool ParticipatesInDamageShare => false;

	public NETHERLAND()
	{
		Intent = 10;
	}

	protected override IEnumerable<AbstractIntent> GetDisplayIntents() => new AbstractIntent[]
	{
		new DollSingleAttackIntent(IntentLocPrefix, () => Intent, () => Intent)
	};

	public override async Task PerformIntent(PlayerChoiceContext choiceContext, bool repeatable = true)
	{
		CombatState? combat = Creature.CombatState;
		Creature? owner = DollOwnerCreature;
		if (combat == null || owner == null)
		{
			return;
		}
		if (combat.HittableEnemies.Count > 0 && Intent > 0)
		{
			await CreatureCmd.Damage(choiceContext, combat.HittableEnemies, Intent, ValueProp.Move, owner, null);
		}
		Intent = Math.Max(0, Intent - 2);
		await TriggerSilk();
		await MaybeRepeatByLube(choiceContext, repeatable);
	}

	public override Task PerformMaxIntent(PlayerChoiceContext choiceContext)
	{
		Intent = 10;
		return Task.CompletedTask;
	}
}

public sealed class RUSSIA : AliceDollMonsterModel
{
	public override int BaseHp => 8;
	public override int MaxCount => 3;

	public RUSSIA()
	{
		Intent = 2;
	}

	protected override IEnumerable<AbstractIntent> GetDisplayIntents() => new AbstractIntent[]
	{
		new DollBuffIntent(IntentLocPrefix, () => Intent)
	};

	public override async Task PerformIntent(PlayerChoiceContext choiceContext, bool repeatable = true)
	{
		CombatState? combat = Creature.CombatState;
		Player? owner = DollOwner;
		if (combat == null || owner == null)
		{
			return;
		}
		int attackerCount = combat.HittableEnemies.Count(e => e.Monster != null && e.Monster.IntendsToAttack);
		if (attackerCount <= 0)
		{
			return;
		}
		foreach (Creature dollCreature in owner.Creature.Pets.Where(p => p.IsAlive && p.Monster is AliceDollMonsterModel && p.Monster is not RUSSIA))
		{
			if (dollCreature.Monster is AliceDollMonsterModel doll)
			{
				doll.Intent += Intent * attackerCount;
			}
		}
		await TriggerSilk();
		await MaybeRepeatByLube(choiceContext, repeatable);
	}

	public override async Task PerformMaxIntent(PlayerChoiceContext choiceContext)
	{
		Player? owner = DollOwner;
		if (owner == null)
		{
			return;
		}
		await PowerCmd.Apply<StrengthPower>(owner.Creature, Intent, owner.Creature, null);
		foreach (Creature dollCreature in owner.Creature.Pets.Where(p => p.IsAlive && p.Monster is AliceDollMonsterModel))
		{
			await PowerCmd.Apply<StrengthPower>(dollCreature, Intent, owner.Creature, null);
		}
	}
}

public sealed class BOMB : AliceDollMonsterModel
{
	public override int BaseHp => 2;
	public override int MaxCount => 12;

	public BOMB()
	{
		Intent = 8;
	}

	protected override IEnumerable<AbstractIntent> GetDisplayIntents() => new AbstractIntent[]
	{
		new DollSingleAttackIntent(IntentLocPrefix, () => Intent, () => Intent)
	};

	public override async Task PerformIntent(PlayerChoiceContext choiceContext, bool repeatable = true)
	{
		CombatState? combat = Creature.CombatState;
		Creature? owner = DollOwnerCreature;
		if (combat != null && owner != null && combat.HittableEnemies.Count > 0)
		{
			await CreatureCmd.Damage(choiceContext, combat.HittableEnemies, Intent, ValueProp.Move, owner, null);
		}
		await CreatureCmd.Kill(Creature, force: true);
	}

	public override Task PerformMaxIntent(PlayerChoiceContext choiceContext)
	{
		Intent *= 2;
		return Task.CompletedTask;
	}
}

public sealed class LONDON : AliceDollMonsterModel
{
	public override int BaseHp => 12;
	public override int MaxCount => 4;

	public LONDON()
	{
		Intent = 3;
	}

	protected override IEnumerable<AbstractIntent> GetDisplayIntents() => new AbstractIntent[]
	{
		new DollDebuffIntent(IntentLocPrefix, () => Intent)
	};

	public override async Task PerformIntent(PlayerChoiceContext choiceContext, bool repeatable = true)
	{
		CombatState? combat = Creature.CombatState;
		Creature? owner = DollOwnerCreature;
		if (combat == null || owner == null)
		{
			return;
		}
		foreach (Creature enemy in combat.Enemies)
		{
			if (enemy.IsAlive)
			{
				await PowerCmd.Apply<PoisonPower>(enemy, Intent, owner, null);
			}
		}
		await TriggerSilk();
		await MaybeRepeatByLube(choiceContext, repeatable);
	}

	public override async Task PerformMaxIntent(PlayerChoiceContext choiceContext)
	{
		CombatState? combat = Creature.CombatState;
		if (combat == null)
		{
			return;
		}
		foreach (Creature enemy in combat.Enemies)
		{
			if (!enemy.IsAlive)
			{
				continue;
			}
			PoisonPower? poison = enemy.GetPower<PoisonPower>();
			if (poison != null)
			{
				await poison.AfterSideTurnStart(enemy.Side, combat);
			}
		}
	}
}

public sealed class CURSE : AliceDollMonsterModel
{
	public override int BaseHp => 6;
	public override int MaxCount => 2;

	public CURSE()
	{
		Intent = 1;
	}

	protected override IEnumerable<AbstractIntent> GetDisplayIntents() => new AbstractIntent[]
	{
		new DollDebuffIntent(IntentLocPrefix, () => Intent)
	};

	public override async Task PerformIntent(PlayerChoiceContext choiceContext, bool repeatable = true)
	{
		CombatState? combat = Creature.CombatState;
		Creature? owner = DollOwnerCreature;
		if (combat == null || owner == null)
		{
			return;
		}
		foreach (Creature enemy in combat.HittableEnemies)
		{
			if (!enemy.IsAlive)
			{
				continue;
			}
			int kinds = ToolBox.GetDebuffKind(enemy);
			if (kinds > 0)
			{
				await PowerCmd.Apply<StrengthPower>(enemy, -kinds, owner, null);
			}
			await PowerCmd.Apply<WeakPower>(enemy, Intent, owner, null);
			await PowerCmd.Apply<VulnerablePower>(enemy, Intent, owner, null);
		}
		await TriggerSilk();
		await MaybeRepeatByLube(choiceContext, repeatable);
	}

	public override async Task PerformMaxIntent(PlayerChoiceContext choiceContext)
	{
		CombatState? combat = Creature.CombatState;
		Creature? owner = DollOwnerCreature;
		if (combat == null || owner == null)
		{
			return;
		}
		foreach (Creature enemy in combat.HittableEnemies)
		{
			if (!enemy.IsAlive)
			{
				continue;
			}
			await PowerCmd.Apply<StrengthPower>(enemy, -1, owner, null);
			int total = ToolBox.GetDebuffTotalCount(enemy);
			if (total > 0)
			{
				await PowerCmd.Apply<StrengthPower>(enemy, -total, owner, null);
			}
		}
	}
}

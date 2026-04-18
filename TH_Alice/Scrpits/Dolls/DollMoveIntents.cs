using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Localization;
using System;
using System.Collections.Generic;

namespace TH_Alice.Scrpits.Dolls;

public sealed class DollSingleAttackIntent : AttackIntent
{
	private readonly string _intentPrefix;
	private readonly Func<decimal> _damageCalc;
	private readonly Func<int> _intentValue;

	protected override string IntentPrefix => _intentPrefix;

	public override int Repeats => 1;

	protected override LocString IntentLabelFormat => new LocString("intents", "FORMAT_DAMAGE_SINGLE");

	public DollSingleAttackIntent(string intentPrefix, Func<decimal> damageCalc, Func<int> intentValue)
	{
		_intentPrefix = intentPrefix;
		_damageCalc = damageCalc;
		_intentValue = intentValue;
		DamageCalc = damageCalc;
	}

	public override int GetTotalDamage(IEnumerable<Creature> targets, Creature owner)
	{
		return GetSingleDamage(targets, owner);
	}

	public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
	{
		LocString intentLabelFormat = IntentLabelFormat;
		intentLabelFormat.Add("Damage", GetTotalDamage(targets, owner));
		return intentLabelFormat;
	}

	public override string GetAnimation(IEnumerable<Creature> targets, Creature owner)
	{
		int totalDamage = GetTotalDamage(targets, owner);
		if (totalDamage < 5)
		{
			return IntentAnimData.attack1;
		}
		if (totalDamage < 10)
		{
			return IntentAnimData.attack2;
		}
		if (totalDamage < 20)
		{
			return IntentAnimData.attack3;
		}
		if (totalDamage < 40)
		{
			return IntentAnimData.attack4;
		}
		return IntentAnimData.attack5;
	}

	protected override LocString GetIntentDescription(IEnumerable<Creature> targets, Creature owner)
	{
		LocString intentDescription = base.GetIntentDescription(targets, owner);
		intentDescription.Add("Intent", _intentValue());
		intentDescription.Add("MaxHealth", owner.MaxHp);
		return intentDescription;
	}

}

public sealed class DollMultiAttackIntent : AttackIntent
{
	private readonly string _intentPrefix;
	private readonly Func<decimal> _damageCalc;
	private readonly Func<int> _repeatCalc;
	private readonly Func<int> _intentValue;

	protected override string IntentPrefix => _intentPrefix;

	protected override LocString IntentLabelFormat => new LocString("intents", "FORMAT_DAMAGE_MULTI");

	public override int Repeats => _repeatCalc();

	public DollMultiAttackIntent(string intentPrefix, Func<decimal> damageCalc, Func<int> repeatCalc, Func<int> intentValue)
	{
		_intentPrefix = intentPrefix;
		_damageCalc = damageCalc;
		_repeatCalc = repeatCalc;
		_intentValue = intentValue;
		DamageCalc = damageCalc;
	}

	public override int GetTotalDamage(IEnumerable<Creature> targets, Creature owner)
	{
		return GetSingleDamage(targets, owner) * Repeats;
	}

	public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
	{
		LocString intentLabelFormat = IntentLabelFormat;
		intentLabelFormat.Add("Damage", GetSingleDamage(targets, owner));
		intentLabelFormat.Add("Repeat", Repeats);
		return intentLabelFormat;
	}

	public override string GetAnimation(IEnumerable<Creature> targets, Creature owner)
	{
		int totalDamage = GetTotalDamage(targets, owner);
		if (totalDamage < 5)
		{
			return IntentAnimData.attack1;
		}
		if (totalDamage < 10)
		{
			return IntentAnimData.attack2;
		}
		if (totalDamage < 20)
		{
			return IntentAnimData.attack3;
		}
		if (totalDamage < 40)
		{
			return IntentAnimData.attack4;
		}
		return IntentAnimData.attack5;
	}

	protected override LocString GetIntentDescription(IEnumerable<Creature> targets, Creature owner)
	{
		LocString intentDescription = base.GetIntentDescription(targets, owner);
		intentDescription.Add("Intent", _intentValue());
		intentDescription.Add("MaxHealth", owner.MaxHp);
		return intentDescription;
	}
}

public sealed class DollDefendIntent : DefendIntent
{
	private readonly string _intentPrefix;
	private readonly Func<int> _intentValue;

	protected override string IntentPrefix => _intentPrefix;

	public DollDefendIntent(string intentPrefix, Func<int> intentValue)
	{
		_intentPrefix = intentPrefix;
		_intentValue = intentValue;
	}

	public override string GetAnimation(IEnumerable<Creature> targets, Creature owner)
	{
		return IntentAnimData.defend;
	}

	protected override LocString GetIntentDescription(IEnumerable<Creature> targets, Creature owner)
	{
		LocString intentDescription = base.GetIntentDescription(targets, owner);
		intentDescription.Add("Intent", _intentValue());
		intentDescription.Add("MaxHealth", owner.MaxHp);
		return intentDescription;
	}
}

public sealed class DollBuffIntent : BuffIntent
{
	private readonly string _intentPrefix;
	private readonly Func<int> _intentValue;

	protected override string IntentPrefix => _intentPrefix;

	public DollBuffIntent(string intentPrefix, Func<int> intentValue)
	{
		_intentPrefix = intentPrefix;
		_intentValue = intentValue;
	}

	public override string GetAnimation(IEnumerable<Creature> targets, Creature owner)
	{
		return IntentAnimData.buff;
	}

	protected override LocString GetIntentDescription(IEnumerable<Creature> targets, Creature owner)
	{
		LocString intentDescription = base.GetIntentDescription(targets, owner);
		intentDescription.Add("Intent", _intentValue());
		intentDescription.Add("MaxHealth", owner.MaxHp);
		return intentDescription;
	}
}

public sealed class DollDebuffIntent : DebuffIntent
{
	private readonly string _intentPrefix;
	private readonly Func<int> _intentValue;

	protected override string IntentPrefix => _intentPrefix;

	public DollDebuffIntent(string intentPrefix, bool strong = false)
		: base(strong)
	{
		_intentPrefix = intentPrefix;
		_intentValue = () => 0;
	}

	public DollDebuffIntent(string intentPrefix, Func<int> intentValue, bool strong = false)
		: base(strong)
	{
		_intentPrefix = intentPrefix;
		_intentValue = intentValue;
	}

	public override string GetAnimation(IEnumerable<Creature> targets, Creature owner)
	{
		return IntentAnimData.debuff;
	}

	protected override LocString GetIntentDescription(IEnumerable<Creature> targets, Creature owner)
	{
		LocString intentDescription = base.GetIntentDescription(targets, owner);
		intentDescription.Add("Intent", _intentValue());
		intentDescription.Add("MaxHealth", owner.MaxHp);
		return intentDescription;
	}
}

public sealed class DollHiddenIntent : AbstractIntent
{
	private readonly string _intentPrefix;
	private readonly Func<int> _intentValue;

	public override IntentType IntentType => IntentType.Hidden;

	protected override string IntentPrefix => _intentPrefix;

	protected override string? SpritePath => null;

	public override bool HasIntentTip => true;

	public DollHiddenIntent(string intentPrefix)
	{
		_intentPrefix = intentPrefix;
		_intentValue = () => 0;
	}

	public DollHiddenIntent(string intentPrefix, Func<int> intentValue)
	{
		_intentPrefix = intentPrefix;
		_intentValue = intentValue;
	}

	public override string GetAnimation(IEnumerable<Creature> targets, Creature owner)
	{
		return IntentAnimData.hidden;
	}

	protected override LocString GetIntentDescription(IEnumerable<Creature> targets, Creature owner)
	{
		LocString intentDescription = base.GetIntentDescription(targets, owner);
		intentDescription.Add("Intent", _intentValue());
		intentDescription.Add("MaxHealth", owner.MaxHp);
		return intentDescription;
	}
}

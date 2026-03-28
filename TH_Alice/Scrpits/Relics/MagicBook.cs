using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH_Alice.Scrpits.Cards;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Main;
using TH_Alice.Scrpits.Powers;
using TH_Alice.TH_Alice.Scrpits.Main;

    [Pool(typeof(AliceRelicPool))]
    public class MagicBook : CustomRelicModel
    {
    public override bool ShowCounter => CombatManager.Instance.IsInProgress;
	private bool _isActive;
	private int SkillsPlayedThisTurn;
      	public override int DisplayAmount
	{
		get
		{
			if (!IsActivating)
			{
				return SkillsPlayedThisTurn % base.DynamicVars.Cards.IntValue;
			}
			return base.DynamicVars.Cards.IntValue;
		}
	}
    	private bool IsActivating
	{
		get
		{
			return _isActive;
		}
		set
		{
			AssertMutable();
			_isActive = value;
			UpdateDisplay();
		}
	}
    	private void UpdateDisplay()
	{
		if (IsActivating)
		{
			base.Status = RelicStatus.Normal;
		}
		else
		{
			int intValue = base.DynamicVars.Cards.IntValue;
			base.Status = ((SkillsPlayedThisTurn % intValue == intValue - 1) ? RelicStatus.Active : RelicStatus.Normal);
		}
		InvokeDisplayAmountChanged();
	}
        protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];

        public override RelicRarity Rarity => RelicRarity.Uncommon;
        public override string PackedIconPath => $"res://ArtWorks/Relics/{Id.Entry}.png";
        protected override string PackedIconOutlinePath => $"res://ArtWorks/Relics/Outlines/{Id.Entry}.png";
        protected override string BigIconPath => $"res://ArtWorks/Relics/{Id.Entry}.png";
        	public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
	{
		if (side != base.Owner.Creature.Side)
		{
			return Task.CompletedTask;
		}
		SkillsPlayedThisTurn = 0;
        UpdateDisplay();
        base.Status = RelicStatus.Normal;
		return Task.CompletedTask;
	}

	public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner == base.Owner && CombatManager.Instance.IsInProgress)
		{
			SkillsPlayedThisTurn++;
			int intValue = base.DynamicVars.Cards.IntValue;
			if (SkillsPlayedThisTurn % intValue == 0)
			{
				await TaskHelper.RunSafely(DoActivateVisuals());
				CardModel card = cardPlay.Card.CreateClone();
                CardCmd.ApplyKeyword(card,CardKeyword.Ethereal);
				await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, addedByPlayer: true);
			}
			UpdateDisplay();
        }
	}
      private async Task DoActivateVisuals()
	{
		IsActivating = true;
		Flash();
		await Cmd.Wait(1f);
		IsActivating = false;
	}

	public override Task AfterCombatEnd(CombatRoom _)
	{
		base.Status = RelicStatus.Normal;
		IsActivating = false;
		return Task.CompletedTask;
	}

    }



using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
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
    public class Fumo : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Rare;
        public override string PackedIconPath => $"res://ArtWorks/Relics/{Id.Entry}.png";
        protected override string PackedIconOutlinePath => $"res://ArtWorks/Relics/Outlines/{Id.Entry}.png";
        protected override string BigIconPath => $"res://ArtWorks/Relics/{Id.Entry}.png";
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
        {
       HoverTipFactory.ForEnergy(this)
        });
        public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
        {
            if (side != base.Owner.Creature.Side)
		{
			return;
		}
		Flash();
           CardModel cardModel = CardFactory.GetDistinctForCombat(Owner.Creature.Player, from c in Owner.Creature.Player.Character.CardPool.GetUnlockedCards(Owner.Creature.Player.UnlockState, Owner.Creature.Player.RunState.CardMultiplayerConstraint)
				where (c.Type == CardType.Power||c.Type == CardType.Attack||c.Type == CardType.Skill)
				select c, 1, Owner.Creature.Player.RunState.Rng.CombatCardGeneration).FirstOrDefault();
			if (cardModel != null)
			{
                cardModel.SetToFreeThisTurn();
                CardCmd.ApplyKeyword(cardModel, CardKeyword.Exhaust);
				await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, addedByPlayer: true);
			}
        }

    }



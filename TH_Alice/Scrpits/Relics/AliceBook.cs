using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
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
    public class AliceBook : CustomRelicModel
    {
        public override bool ShowCounter => false;
        public override RelicRarity Rarity => RelicRarity.Event;
        public override string PackedIconPath => $"res://ArtWorks/Relics/{Id.Entry}.png";
        protected override string PackedIconOutlinePath => $"res://ArtWorks/Relics/Outlines/{Id.Entry}.png";
        protected override string BigIconPath => $"res://ArtWorks/Relics/{Id.Entry}.png";
       public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
	{
		if (side == base.Owner.Creature.Side && combatState.RoundNumber <= 1)
		{
			CardModel cardModel;
            List<CardModel> cards = CardFactory.GetDistinctForCombat(Owner,base.Owner.Character.CardPool.GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint), base.Owner.Character.CardPool.GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint).Count(),Owner.RunState.Rng.CombatCardGeneration).ToList();
            CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 0,1);
            cardModel = (await CardSelectCmd.FromSimpleGrid(choiceContext, cards, base.Owner, prefs)).FirstOrDefault();
            if (cardModel != null)
            {
                CardCmd.Upgrade(cardModel);
                await CardPileCmd.Add(Owner.RunState.CloneCard(cardModel), PileType.Deck);
                await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Draw, addedByPlayer: true);
            }
		}
	}

    }
    



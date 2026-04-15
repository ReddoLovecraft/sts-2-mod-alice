using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Main;
using TH_Alice.Scrpits.Powers;
using TH_Alice.TH_Alice.Scrpits.Main;
using static MegaCrit.Sts2.Core.Models.Monsters.KnowledgeDemon;

namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(AliceCardPool))]
public class DollMake : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    static string text = StringHelper.Slugify("Doll");
    static LocString locString = ToolBox.L10NStatic(text + ".title");
    static LocString locString2 = ToolBox.L10NStatic(text + ".description");
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
        new HoverTip(locString,locString2)
  });
    public DollMake() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.None)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
            CardModel cardModel;
            List<CardModel> cards =[ModelDb.Card<CreatePengLai>().ToMutable(), ModelDb.Card<CreateShangHai>().ToMutable(), ModelDb.Card<CreateNetherland>().ToMutable(), ModelDb.Card<CreateBomb>().ToMutable(), ModelDb.Card<CreateCurse>().ToMutable(), ModelDb.Card<CreateRussia>().ToMutable(), ModelDb.Card<CreateXiZang>().ToMutable(), ModelDb.Card<CreateGoliath>().ToMutable(), ModelDb.Card<CreateRoundTable>().ToMutable(), ModelDb.Card<CreateHina>().ToMutable(), ModelDb.Card<CreateLondon>().ToMutable(), ModelDb.Card<CreateOrl>().ToMutable(), ModelDb.Card<CreateFrance>().ToMutable()];
           // List<CardModel> cards = CardFactory.GetDistinctForCombat(Owner,c,13, Owner.RunState.Rng.CombatCardGeneration).ToList();
        for (int i=0;i< base.DynamicVars.Cards.IntValue;i++)
        {
            CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 1);
            cardModel = (await CardSelectCmd.FromSimpleGrid(choiceContext, cards, base.Owner, prefs)).FirstOrDefault();
             if(Owner.Character is AliceCharacter)
            {
                await CreatureCmd.TriggerAnim(base.Owner.Creature, "Summon", base.Owner.Character.CastAnimDelay);
            }
            if (cardModel != null)
            {
                cardModel.Owner=this.Owner;
                await ((AliceCardModel)cardModel).OnChosen();
            }
        }
    }
    protected override void OnUpgrade()
    {
       base.DynamicVars.Cards.UpgradeValueBy(1);
    }
}

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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
public class DollCreate : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    static string text = StringHelper.Slugify("Doll");
    static LocString locString = ToolBox.L10NStatic(text + ".title");
    static LocString locString2 = ToolBox.L10NStatic(text + ".description");
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
        new HoverTip(locString,locString2)
  });
    public DollCreate() : base(1, CardType.Skill, CardRarity.Basic, TargetType.None)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        //创建上海人形
        //加入随机逻辑
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
        if (!this.IsUpgraded)
        {
            await CreatureCmd.TriggerAnim(base.Owner.Creature, "Summon", base.Owner.Character.CastAnimDelay);
            await ToolBox.MakeRandomDoll(Owner.Creature);
        }
        else 
        {
            CardModel cardModel;
             IEnumerable<CardModel> c =[ModelDb.Card<CreatePengLai>(), ModelDb.Card<CreateShangHai>(), ModelDb.Card<CreateNetherland>(), ModelDb.Card<CreateBomb>(), ModelDb.Card<CreateCurse>(), ModelDb.Card<CreateRussia>(), ModelDb.Card<CreateXiZang>(), ModelDb.Card<CreateGoliath>(), ModelDb.Card<CreateRoundTable>(), ModelDb.Card<CreateHina>(), ModelDb.Card<CreateLondon>(), ModelDb.Card<CreateOrl>(), ModelDb.Card<CreateFrance>()];
            List<CardModel> cards = CardFactory.GetDistinctForCombat(Owner,c,3, Owner.RunState.Rng.CombatCardGeneration).ToList();
            cardModel = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, base.Owner, canSkip: false);
            await CreatureCmd.TriggerAnim(base.Owner.Creature, "Summon", base.Owner.Character.CastAnimDelay);
            if (cardModel != null)
            {
                await ((AliceCardModel)cardModel).OnChosen();
            }

        }

        

    }
    protected override void OnUpgrade()
    {
       
    }
}

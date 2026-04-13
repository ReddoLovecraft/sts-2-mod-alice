using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Dolls;
using TH_Alice.Scrpits.Main;
using TH_Alice.Scrpits.Powers;
using TH_Alice.TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(AliceCardPool))]
public class DollCremation : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];
    static string text = StringHelper.Slugify("Doll");
    static LocString locString = ToolBox.L10NStatic(text + ".title");
    static LocString locString2 = ToolBox.L10NStatic(text + ".description");
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
  {
        new HoverTip(locString,locString2),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
  });
    public DollCremation() : base(0, CardType.Skill, CardRarity.Common, TargetType.None)
    {

    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
       CardPile pile = PileType.Hand.GetPile(base.Owner);
		CardModel cardModel2 = base.Owner.RunState.Rng.CombatCardSelection.NextItem(pile.Cards);
		if (cardModel2 != null)
		{
			await CardCmd.Exhaust(choiceContext, cardModel2);
		}
    if(IsUpgraded)
    {
         CardPile pile2 = PileType.Hand.GetPile(base.Owner);
		CardModel cardModel3 = base.Owner.RunState.Rng.CombatCardSelection.NextItem(pile2.Cards);
		if (cardModel3 != null)
		{
			await CardCmd.Exhaust(choiceContext, cardModel3);
		}
    }

        for(int i=0;i<base.DynamicVars.Cards.IntValue;i++)
        {
           if (base.Owner.Character is AliceCharacter)
		            {
			        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Summon", base.Owner.Character.CastAnimDelay);
		            }
        await ToolBox.MakeRandomDoll(Owner.Creature);
        }
    }
    protected override void OnUpgrade()
    {
      base.DynamicVars.Cards.UpgradeValueBy(1);
    }
}

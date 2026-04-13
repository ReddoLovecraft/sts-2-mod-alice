using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Main;
using TH_Alice.Scrpits.Powers;
using TH_Alice.TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(AliceCardPool))]
public class MoreAndMore : AliceCardModel
{
    protected override bool ShouldGlowGoldInternal => ToolBox.GetDollCount(Owner.Creature) == 0;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
       new CardsVar(3)
     ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    static string text = StringHelper.Slugify("Doll");
    static LocString locString = ToolBox.L10NStatic(text + ".title");
    static LocString locString2 = ToolBox.L10NStatic(text + ".description");
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
        new HoverTip(locString,locString2)
  });
    public MoreAndMore() : base(1, CardType.Skill ,CardRarity.Rare, TargetType.None)
	{
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{

       if(ToolBox.GetDollCount(Owner.Creature)<=0)
       {
          if (base.Owner.Character is AliceCharacter)
		    {
			 await CreatureCmd.TriggerAnim(base.Owner.Creature, "Summon", base.Owner.Character.CastAnimDelay);
		    }
            await ToolBox.MakeRandomDoll(Owner.Creature);
             if (base.Owner.Character is AliceCharacter)
		    {
			 await CreatureCmd.TriggerAnim(base.Owner.Creature, "Summon", base.Owner.Character.CastAnimDelay);
		    }
            await ToolBox.MakeRandomDoll(Owner.Creature);
       }
       else
       {
          int totalDoll = ToolBox.GetDollCount(Owner.Creature);
          int repeatCnt= (int)(totalDoll /base.DynamicVars.Cards.BaseValue);
          for(int i=0;i<repeatCnt;i++)
          {
             if (base.Owner.Character is AliceCharacter)
		    {
			 await CreatureCmd.TriggerAnim(base.Owner.Creature, "Summon", base.Owner.Character.CastAnimDelay);
		    }
            await ToolBox.MakeRandomDoll(Owner.Creature);
          }
       }
    }
	protected override void OnUpgrade()
	{
      base.DynamicVars.Cards.UpgradeValueBy(-1);
  }
}

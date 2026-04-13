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
public class Iteration : AliceCardModel
{
    protected override bool ShouldGlowGoldInternal => ToolBox.GetDollCount(Owner.Creature)>=base.DynamicVars.Cards.IntValue;
    protected override bool IsPlayable => ToolBox.GetDollCount(Owner.Creature)>0;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new CardsVar(2)
     ];
    static string text = StringHelper.Slugify("Doll");
    static LocString locString = ToolBox.L10NStatic(text + ".title");
    static LocString locString2 = ToolBox.L10NStatic(text + ".description");
    static string text2 = StringHelper.Slugify("Recycle");
    static LocString locString3 = ToolBox.L10NStatic(text2 + ".title");
    static LocString locString4 = ToolBox.L10NStatic(text2 + ".description");
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
  {
        new HoverTip(locString3,locString4),
        new HoverTip(locString,locString2)
  });
    public Iteration() : base(0, CardType.Skill ,CardRarity.Common, TargetType.None)
	{
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        int cnt = ToolBox.GetDollCount(Owner.Creature);
        if(cnt>0&&cnt< base.DynamicVars.Cards.IntValue) 
        {
           await ToolBox.RecycleDolls(Owner.Creature,cnt);
           for(int i = 0;i<cnt;i++)
           {
             if (base.Owner.Character is AliceCharacter)
		            {
			        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Summon", base.Owner.Character.CastAnimDelay);
		            }
                await ToolBox.MakeRandomDoll(Owner.Creature);
           }
        }
        else if(cnt>0&&cnt>=base.DynamicVars.Cards.IntValue)
        {
            await ToolBox.RecycleDolls(Owner.Creature, base.DynamicVars.Cards.IntValue);
            for (int i = 0; i < base.DynamicVars.Cards.IntValue; i++)
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
        base.DynamicVars.Cards.UpgradeValueBy(1);
    }
}

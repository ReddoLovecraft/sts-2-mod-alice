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
public class DollWax : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new CardsVar(1)
     ];
    static string text = StringHelper.Slugify("Doll");
    static LocString locString = ToolBox.L10NStatic(text + ".title");
    static LocString locString2 = ToolBox.L10NStatic(text + ".description");
    static string text2 = StringHelper.Slugify("Wax");
    static LocString locString3 = ToolBox.L10NStatic(text2 + ".title");
    static LocString locString4 = ToolBox.L10NStatic(text2 + ".description");
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
  {
         new HoverTip(locString3,locString4),
        new HoverTip(locString,locString2)
  });
    public DollWax() : base(1, CardType.Skill ,CardRarity.Common, TargetType.None)
	{
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.IntValue,Owner.Creature.Player);
         if (base.Owner.Character is AliceCharacter)
		            {
			        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Summon", base.Owner.Character.CastAnimDelay);
		            }
        await ToolBox.MakeRandomDoll(Owner.Creature,true);
    }
	protected override void OnUpgrade()
	{
        base.DynamicVars.Cards.UpgradeValueBy(1);
    }
}

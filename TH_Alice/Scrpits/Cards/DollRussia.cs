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
public class DollRussia : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new DynamicVar("Power", 3)
     ];
    static string text = StringHelper.Slugify("Doll");
    static LocString locString = ToolBox.L10NStatic(text + ".title");
    static LocString locString2 = ToolBox.L10NStatic(text + ".description");
    static string text2 = StringHelper.Slugify("Russia");
    static LocString locString3 = ToolBox.L10NStatic(text2 + ".title");
    static LocString locString4 = ToolBox.L10NStatic(text2 + ".description");
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[3]
  {
         new HoverTip(locString3,locString4),
        new HoverTip(locString,locString2),
        HoverTipFactory.FromPower<StrengthPower>()
  });
    public DollRussia() : base(1, CardType.Skill ,CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        await ToolBox.MakeDoll<RussiaPower>(Owner.Creature);
        if(cardPlay.Target.IsAlive&&cardPlay.Target.Monster.IntendsToAttack)
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, base.DynamicVars["Power"].IntValue, base.Owner.Creature, this);
    }
	protected override void OnUpgrade()
	{
        base.DynamicVars["Power"].UpgradeValueBy(2);
    }
}

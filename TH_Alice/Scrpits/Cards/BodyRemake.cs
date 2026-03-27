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
public class BodyRemake : AliceCardModel
{
   public override bool CanBeGeneratedInCombat => false;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust,CardKeyword.Ethereal];
    protected override bool IsPlayable => ToolBox.GetDollCount(Owner.Creature)>0;
     protected override bool ShouldGlowGoldInternal =>ToolBox.GetDollCount(Owner.Creature)>0;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
       new MaxHpVar(2)
     ];
    static string text = StringHelper.Slugify("Doll");
    static LocString locString = ToolBox.L10NStatic(text + ".title");
    static LocString locString2 = ToolBox.L10NStatic(text + ".description");
   static string text2 = StringHelper.Slugify("Recycle");
    static LocString locString3 = ToolBox.L10NStatic(text2 + ".title");
    static LocString locString4 = ToolBox.L10NStatic(text2 + ".description");
      protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
  {
          new HoverTip(locString,locString2),
          new HoverTip(locString3,locString4)
  });
    public BodyRemake() : base(3, CardType.Skill ,CardRarity.Rare, TargetType.None)
	{
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
       int increasement=ToolBox.GetDollCount(Owner.Creature)*2;
       await ToolBox.RecycleDolls(Owner.Creature,ToolBox.GetDollCount(Owner.Creature));
       await CreatureCmd.GainMaxHp(Owner.Creature,increasement);
  }
	protected override void OnUpgrade()
	{
    RemoveKeyword(CardKeyword.Ethereal);
  }
}

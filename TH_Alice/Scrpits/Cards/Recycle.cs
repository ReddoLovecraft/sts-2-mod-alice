using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
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
public class Recycle : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
     ];
    static string text2 = StringHelper.Slugify("Recycle");
    static LocString locString3 = ToolBox.L10NStatic(text2 + ".title");
    static LocString locString4 = ToolBox.L10NStatic(text2 + ".description");
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
  {
         new HoverTip(locString3,locString4),
         HoverTipFactory.FromCard<DollPart>()
  });
    public Recycle() : base(1, CardType.Skill ,CardRarity.Uncommon, TargetType.None)
	{
	}
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        List<CardModel> list = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 0, 999), context: choiceContext, player: base.Owner, filter: null, source: this)).ToList();
        foreach (CardModel item in list)
        {
            CardModel cardModel = base.CombatState.CreateCard<DollPart>(base.Owner);
            if (base.IsUpgraded)
            {
                CardCmd.Upgrade(cardModel);
            }
            await CardCmd.Transform(item, cardModel);
        }
    }
	protected override void OnUpgrade()
	{
        AddKeyword(CardKeyword.Retain);
    }
}

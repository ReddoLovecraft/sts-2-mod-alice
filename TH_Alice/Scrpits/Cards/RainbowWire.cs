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
public class RainbowWire : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new IntVar("Replay", 2)
     ];
   
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
       HoverTipFactory.Static(StaticHoverTip.ReplayStatic)
  });
    public RainbowWire() : base(1, CardType.Skill ,CardRarity.Rare, TargetType.None)
	{
	}
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        List<CardModel> list = PileType.Hand.GetPile(base.Owner).Cards.ToList();
        if (list.Count == 0)
        {
            return;
        }
        List<CardModel> list2 = list.Where(delegate (CardModel c)
        {
            bool flag = !c.Keywords.Contains(CardKeyword.Unplayable);
            bool flag2 = flag;
            if (flag2)
            {
                CardType type = c.Type;
                bool flag3 = (uint)(type - 5) <= 1u;
                flag2 = !flag3;
            }
            return flag2;
        }).ToList();
        List<CardModel> list3 = list2.Where(delegate (CardModel c)
        {
            CardType type = c.Type;
            return (uint)(type - 1) <= 2u;
        }).ToList();
        IEnumerable<CardModel> items = ((list3.Count == 0) ? list2 : list3);
        CardModel cardModel = base.Owner.RunState.Rng.CombatCardSelection.NextItem(items);
        if (cardModel != null)
        {
            cardModel.BaseReplayCount += base.DynamicVars["Replay"].IntValue;
            CardCmd.Preview(cardModel);
        }
    }
	protected override void OnUpgrade()
	{
        base.DynamicVars["Replay"].UpgradeValueBy(1);
    }
}

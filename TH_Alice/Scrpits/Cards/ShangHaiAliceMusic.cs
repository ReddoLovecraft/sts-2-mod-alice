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
public class ShangHaiAliceMusic : AliceCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Ethereal];
    public ShangHaiAliceMusic() : base(2, CardType.Skill ,CardRarity.Rare, TargetType.None)
	{
	}
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        foreach (CardModel card in PileType.Hand.GetPile(base.Owner).Cards)
		{
			card.EnergyCost.AddThisCombat(-1);
     	CardCmd.ApplyKeyword(card, CardKeyword.Ethereal);
		}
    foreach (CardModel card in PileType.Discard.GetPile(base.Owner).Cards)
		{
			card.EnergyCost.AddThisCombat(-1);
     	CardCmd.ApplyKeyword(card, CardKeyword.Ethereal);
		}
    foreach (CardModel card in PileType.Draw.GetPile(base.Owner).Cards)
		{
			card.EnergyCost.AddThisCombat(-1);
     	CardCmd.ApplyKeyword(card, CardKeyword.Ethereal);
		}
   }
	protected override void OnUpgrade()
	{
   this.EnergyCost.UpgradeBy(-1);
  }
}

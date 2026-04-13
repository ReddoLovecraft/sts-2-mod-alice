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
public class DollInSea : AliceCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain,CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new CardsVar(1)
     ];
  
    public DollInSea() : base(1, CardType.Skill ,CardRarity.Uncommon, TargetType.None)
	{
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
         await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 1);
        CardPile pile = PileType.Discard.GetPile(base.Owner);
        CardModel cardModel = (await CardSelectCmd.FromSimpleGrid(choiceContext, pile.Cards, base.Owner, prefs)).FirstOrDefault();
        if (cardModel != null)
        {
            cardModel.SetToFreeThisTurn();
            await CardPileCmd.Add(cardModel, PileType.Hand);
        }
    }
	protected override void OnUpgrade()
	{
        RemoveKeyword(CardKeyword.Exhaust);
    }
}

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
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
public class MessageExchange : AliceCardModel
{
   private CardModel? _mockGeneratedCard;
    public MessageExchange() : base(1, CardType.Skill ,CardRarity.Uncommon, TargetType.None)
	{
	}

    	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		CardModel cardModel;
		if (_mockGeneratedCard == null)
		{
			List<CardPoolModel> list = base.Owner.UnlockState.CharacterCardPools.ToList();
			if (list.Count > 1)
			{
				list.Remove(base.Owner.Character.CardPool);
			}
			IEnumerable<CardModel> cards = from c in list.SelectMany((CardPoolModel c) => c.GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint))
				where c.Type == CardType.Skill
				select c;
			List<CardModel> list2 = CardFactory.GetDistinctForCombat(base.Owner, cards, 3, base.Owner.RunState.Rng.CombatCardGeneration).ToList();
			if (base.IsUpgraded)
			{
				foreach (CardModel item in list2)
				{
					CardCmd.Upgrade(item);
				}
			}
			cardModel = await CardSelectCmd.FromChooseACardScreen(choiceContext, list2, base.Owner, canSkip: true);
		}
		else
		{
			cardModel = _mockGeneratedCard;
			if (base.IsUpgraded)
			{
				CardCmd.Upgrade(cardModel);
			}
		}
		if (cardModel != null)
		{
			cardModel.SetToFreeThisTurn();
			await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, addedByPlayer: true);
		}
	}
	public void MockGeneratedCard(CardModel card)
	{
		AssertMutable();
		_mockGeneratedCard = card;
	}
	protected override void OnUpgrade()
	{
     base.EnergyCost.UpgradeBy(-1);
  }
}

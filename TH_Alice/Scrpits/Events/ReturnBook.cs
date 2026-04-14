using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Events;
public sealed class ReturnBook : CustomEventModel
{
    private CardModel? _randomCardToLose;
    private CardModel? RandomCardToLose
	{
		get
		{
			return _randomCardToLose;
		}
		set
		{
			AssertMutable();
			_randomCardToLose = value;
		}
	}
    public override string? CustomInitialPortraitPath => "res://ArtWorks/Events/returnbook.png";
 
	private EventOption CreateOption(Func<Task>? onChosen, string optionKey, IEnumerable<IHoverTip>? hoverTips = null)
	{
		LocString title = new LocString(LocTable, optionKey + ".title");
		LocString description = new LocString(LocTable, optionKey + ".description");
		return new EventOption(this, onChosen, title, description, optionKey, hoverTips ?? Enumerable.Empty<IHoverTip>());
	}

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar("CardStolen"),
        new HpLossVar(10)
    ];
   	public override bool IsAllowed(RunState runState)
	{
		if (runState.TotalFloor > 6)
		{
			return runState.Players.All((Player p) => p.Deck.Cards.Any((CardModel c) => c.IsRemovable&&c.Rarity==CardRarity.Rare));
		}
		return false;
	}
    protected override Task BeforeEventStarted()
    {
        Owner!.CanRemovePotions = false;
        return Task.CompletedTask;
    }
    protected override void OnEventFinished()
    {
        Owner!.CanRemovePotions = true;
    }
    private void GetNewRandomCard()
	{
		List<CardModel> list = ((RandomCardToLose != null) ? base.Owner.Deck.Cards.Where((CardModel c) => c.GetType() != RandomCardToLose.GetType()).ToList() : base.Owner.Deck.Cards.Where((CardModel c) => c.Rarity ==CardRarity.Rare
        ).ToList());
		list.RemoveAll((CardModel c) => !c.IsRemovable);
		if (list.Count == 0)
		{
			list = base.Owner.Deck.Cards.Where((CardModel c) => c.IsRemovable).ToList();
		}
		RandomCardToLose = base.Rng.NextItem(list);
		StringVar stringVar = (StringVar)base.DynamicVars["CardStolen"];
		stringVar.StringValue = RandomCardToLose.Title;
	}
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        GetNewRandomCard();
        return (new EventOption[3] {
            CreateOption(Back, "TH_ALICE-RETURN_BOOK.pages.INITIAL.options.BACK",[HoverTipFactory.FromCard(RandomCardToLose)]),
            CreateOption(Remove, "TH_ALICE-RETURN_BOOK.pages.INITIAL.options.REMOVE"),
            CreateOption(Run, "TH_ALICE-RETURN_BOOK.pages.INITIAL.options.RUN")
    });
    }
    
    private async Task Run()
    {
        await CardPileCmd.RemoveFromDeck(RandomCardToLose);
        SetEventFinished(PageDescription("RUN"));
    }
    private async Task Back()
    {
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner.Creature, base.DynamicVars.HpLoss.IntValue, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        SetEventFinished(PageDescription("BACK")); 
    }

     private async Task Remove()
    {
        await CardPileCmd.RemoveFromDeck(RandomCardToLose);
        await CardPileCmd.RemoveFromDeck((await CardSelectCmd.FromDeckForRemoval(base.Owner, new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 2))).ToList());
        SetEventFinished(PageDescription("REMOVE"));
    }
}

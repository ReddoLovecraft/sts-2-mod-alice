using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
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
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Events;
public sealed class MeetMengzi : CustomEventModel
{
    public override string? CustomInitialPortraitPath => "res://ArtWorks/Events/meetmengzi.png";
    private PotionModel? _drinkAndLiftPotion;
    private PotionModel? DrinkAndLiftPotion
	{
		get
		{
			return _drinkAndLiftPotion;
		}
		set
		{
			AssertMutable();
			_drinkAndLiftPotion = value;
		}
	}

	private EventOption CreateOption(Func<Task>? onChosen, string optionKey, IEnumerable<IHoverTip>? hoverTips = null)
	{
		LocString title = new LocString(LocTable, optionKey + ".title");
		LocString description = new LocString(LocTable, optionKey + ".description");
		return new EventOption(this, onChosen, title, description, optionKey, hoverTips ?? Enumerable.Empty<IHoverTip>());
	}

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar("DrinkRandomPotion"),
        new GoldVar(100)
    ];
   public override bool IsAllowed(RunState runState)
	{
		if (runState.CurrentActIndex >1)
		{
            bool flag=false;
            foreach (var player in runState.Players)
            {
                if (player.Character != null && player.Character is AliceCharacter)
                {
                    flag=true;
                }
            }
			return flag;
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
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        DrinkAndLiftPotion = base.Rng.NextItem(Owner!.Potions);
        EventOption drink;
		if (DrinkAndLiftPotion != null)
		{
			StringVar stringVar = (StringVar)base.DynamicVars["DrinkRandomPotion"];
			stringVar.StringValue = DrinkAndLiftPotion.Title.GetFormattedText();
            drink = CreateOption(Drink, "TH_ALICE-MEET_MENGZI.pages.INITIAL.options.DRINK", new IHoverTip[] { HoverTipFactory.FromPotion(DrinkAndLiftPotion) });
		}
        else
		{
			drink = CreateOption(null, "TH_ALICE-MEET_MENGZI.pages.INITIAL.options.DRINK_LOCKED");
		}
        EventOption buy;
        if(Owner.Gold>=DynamicVars.Gold.BaseValue)
        {
            buy = CreateOption(Buy, "TH_ALICE-MEET_MENGZI.pages.INITIAL.options.BUY");
        }
        else
        {
            buy = CreateOption(null, "TH_ALICE-MEET_MENGZI.pages.INITIAL.options.BUY_LOCKED");
        }
        return (new EventOption[3] { drink, buy , CreateOption(Curse, "TH_ALICE-MEET_MENGZI.pages.INITIAL.options.BACK", HoverTipFactory.FromCardWithCardHoverTips<Writhe>())});
    }
    private async Task Drink()
    {
       await PotionCmd.Discard(DrinkAndLiftPotion);
       AwakePage();
    }
    private async Task Buy()
    {
        await PlayerCmd.LoseGold(DynamicVars.Gold.BaseValue, Owner!, GoldLossType.Stolen);
        AwakePage();
    }
    //
      private async Task Curse()
    {
        await CardPileCmd.AddCursesToDeck(Enumerable.Repeat(ModelDb.Card<Writhe>(),1), base.Owner);
        SetEventFinished(PageDescription("BACK"));
    }
    private void AwakePage()
    {
        SetEventState(PageDescription("AWAKE"), [
            CreateOption(ChooseRelic, "TH_ALICE-MEET_MENGZI.pages.AWAKE.options.RELIC"), 
            CreateOption(ChooseGold, "TH_ALICE-MEET_MENGZI.pages.AWAKE.options.GOLD"),
        ]);
    }

    
    private async Task ChooseRelic()
    {
        RelicModel relic = RelicFactory.PullNextRelicFromFront(base.Owner, RelicRarity.Rare).ToMutable();
		await RelicCmd.Obtain(relic, base.Owner);
        RelicModel relic2 = RelicFactory.PullNextRelicFromFront(base.Owner, RelicRarity.Rare).ToMutable();
		await RelicCmd.Obtain(relic2, base.Owner);
        SetEventFinished(PageDescription("RELIC")); 
    }
    private async Task ChooseGold()
    {
       await PlayerCmd.GainGold(300, base.Owner);
        SetEventFinished(PageDescription("GOLD"));
    }
}

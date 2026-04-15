using System.ComponentModel;
using BaseLib.Abstracts;
using HarmonyLib;
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
using TH_Alice.Scrpits.Cards;
using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Events;
public sealed class MeetSakuya : CustomEventModel
{
    public override string? CustomInitialPortraitPath => "res://ArtWorks/Events/meetsakuya.png";
    private RelicModel randomrelic;
	private EventOption CreateOption(Func<Task>? onChosen, string optionKey, IEnumerable<IHoverTip>? hoverTips = null)
	{
		LocString title = new LocString(LocTable, optionKey + ".title");
		LocString description = new LocString(LocTable, optionKey + ".description");
		return new EventOption(this, onChosen, title, description, optionKey, hoverTips ?? Enumerable.Empty<IHoverTip>());
	}

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
       new StringVar("ExchangeRelic"),
       new StringVar("RandomRelic")
    ];
    public override bool IsAllowed(RunState runState) 
    {
		if (runState.Players.Any((Player p) => !GetValidRelics(p).Any()))
		{
			return false;
		}
        bool flag=true;
        foreach (var player in runState.Players)
        {
            if (player.Character != null && player.Character is not AliceCharacter)
             {
                    flag=false;
                    break;
             }
        }
		return flag;
    }
    	private IEnumerable<RelicModel> GetValidRelics(Player player)
	{
		return player.Relics.Where((RelicModel r) => r.IsTradable);
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
    private LocString RelicChoiceTitle => new LocString("events", "TH_ALICE-MEET_SAKUYA.pages.INITIAL.options.EXCHANGE_RELIC.title");
    private LocString RelicChoiceDescription => new LocString("events", "TH_ALICE-MEET_SAKUYA.pages.INITIAL.options.EXCHANGE_RELIC.description");
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        List<EventOption> list = new List<EventOption>();
        	RelicModel relic = base.Rng.NextItem(base.Owner.Relics.Where((RelicModel r) => r.IsTradable));
		if (relic != null)
		{
            randomrelic=RelicFactory.PullNextRelicFromFront(base.Owner).ToMutable();
			((StringVar)base.DynamicVars["ExchangeRelic"]).StringValue = relic.Title.GetFormattedText();
			((StringVar)base.DynamicVars["RandomRelic"]).StringValue = randomrelic.Title.GetFormattedText();
            list.Add( CreateOption(async delegate
			{
				await ExchangeRelic(relic);
			}, "TH_ALICE-MEET_SAKUYA.pages.INITIAL.options.EXCHANGE_RELIC",new IHoverTip[] { relic.HoverTip ,randomrelic.HoverTip}));
            list.Add( CreateOption(async delegate
			{
				await ExchangeGold(relic);
			}, "TH_ALICE-MEET_SAKUYA.pages.INITIAL.options.EXCHANGE_GOLD",  new IHoverTip[] { relic.HoverTip }));
		}
		else
		{
			list.Add( CreateOption(null, "TH_ALICE-MEET_SAKUYA.pages.INITIAL.options.EXCHANGE_RELIC_LOCKED"));
            list.Add( CreateOption(null, "TH_ALICE-MEET_SAKUYA.pages.INITIAL.options.EXCHANGE_GOLD_LOCKED"));
		}
        list.Add( CreateOption(Learn, "TH_ALICE-MEET_SAKUYA.pages.INITIAL.options.LEARN",new IHoverTip[] { HoverTipFactory.FromCard<SakuyaDoll>() }));
        return list;
    }
    private async Task ExchangeRelic(RelicModel relic)
    {
        await RelicCmd.Remove(relic);
		await RelicCmd.Obtain(randomrelic, base.Owner);
        SetEventFinished(PageDescription("EXCHANGE_RELIC")); 
    }
    private async Task ExchangeGold(RelicModel relic)
    {
        await RelicCmd.Remove(relic);
        await PlayerCmd.GainGold(150, Owner!);
        SetEventFinished(PageDescription("EXCHANGE_GOLD"));
    }
     private async Task Learn()
    {
       CardModel card = base.Owner.RunState.CreateCard<SakuyaDoll>(base.Owner);
		CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck), 2f);
        SetEventFinished(PageDescription("LEARN"));
    }
}

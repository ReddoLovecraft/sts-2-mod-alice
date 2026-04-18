using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
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
public sealed class DollPerform : CustomEventModel
{
    public override string? CustomInitialPortraitPath => "res://ArtWorks/Events/dollperform.png";
	private EventOption CreateOption(Func<Task>? onChosen, string optionKey, IEnumerable<IHoverTip>? hoverTips = null)
	{
		LocString title = new LocString(LocTable, optionKey + ".title");
		LocString description = new LocString(LocTable, optionKey + ".description");
		return new EventOption(this, onChosen, title, description, optionKey, hoverTips ?? Enumerable.Empty<IHoverTip>());
	}
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new GoldVar(200)
    ];
    protected override Task BeforeEventStarted(bool isPreFinished)
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
        return (new EventOption[2] {CreateOption(Join, "TH_ALICE-DOLL_PERFORM.pages.INITIAL.options.JOIN"),CreateOption(Watch, "TH_ALICE-DOLL_PERFORM.pages.INITIAL.options.WATCH")});
    }
      public override bool IsAllowed(IRunState runState)
	{
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
    private async Task Watch()
    {
        IEnumerable<CardModel> enumerable = PileType.Deck.GetPile(base.Owner).Cards.Where((CardModel c) => c?.IsUpgradable ?? false).ToList().StableShuffle(base.Owner.RunState.Rng.Niche)
				.Take(3);
		foreach (CardModel item in enumerable)
		{
			CardCmd.Upgrade(item);
		}
        SetEventFinished(PageDescription("WATCH")); 
    }
    private async Task Join()
    {
        await PlayerCmd.GainGold(200, Owner!);
        SetEventFinished(PageDescription("JOIN"));
    }
}

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
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
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Events;
public sealed class PastFuture : CustomEventModel
{
    public override string? CustomInitialPortraitPath => "res://ArtWorks/Events/pastfuture.png";
	private EventOption CreateOption(Func<Task>? onChosen, string optionKey, IEnumerable<IHoverTip>? hoverTips = null)
	{
		LocString title = new LocString(LocTable, optionKey + ".title");
		LocString description = new LocString(LocTable, optionKey + ".description");
		return new EventOption(this, onChosen, title, description, optionKey, hoverTips ?? Enumerable.Empty<IHoverTip>());
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
        return (new EventOption[2] 
        {CreateOption(Back, "TH_ALICE-PAST_FUTURE.pages.INITIAL.options.PAST",HoverTipFactory.FromRelic<AliceBook>()),
        CreateOption(Ahead, "TH_ALICE-PAST_FUTURE.pages.INITIAL.options.FUTURE",new IHoverTip[] { HoverTipFactory.Static(StaticHoverTip.Transform)})});
    }
    
    private async Task Ahead()
    {
     	List<CardModel> list = (await CardSelectCmd.FromDeckForTransformation(prefs: new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 5), player: base.Owner)).ToList();
		foreach (CardModel item in list)
		{
			CardModel cardModel = CardFactory.CreateRandomCardForTransform(item, isInCombat: false, base.Owner.RunState.Rng.Niche);
			CardCmd.Upgrade(cardModel);
			await CardCmd.Transform(item, cardModel);
		}
        SetEventFinished(PageDescription("FUTURE")); 
    }
    private async Task Back()
    {
        RelicModel relic = ModelDb.Relic<AliceBook>().ToMutable();
		await RelicCmd.Obtain(relic, base.Owner);
        SetEventFinished(PageDescription("PAST"));
    }
}

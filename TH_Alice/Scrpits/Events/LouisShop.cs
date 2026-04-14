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
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Events;
public sealed class LouisShop : CustomEventModel
{
    public override string? CustomInitialPortraitPath => "res://ArtWorks/Events/louisshop.png";
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
        new GoldVar(200)
    ];
    public override bool IsAllowed(RunState runState) => runState.Players.All(p => p.Gold >= DynamicVars.Gold.BaseValue);
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
        EventOption buyRelic;
        if(Owner.Gold>=300)
         {
            buyRelic = CreateOption(BuyRelic, "TH_ALICE-LOUIS_SHOP.pages.INITIAL.options.BUY_RELIC");
        }
        else
        {
            buyRelic = CreateOption(null, "TH_ALICE-LOUIS_SHOP.pages.INITIAL.options.BUY_RELIC_LOCKED");
        }
        return (new EventOption[3] {CreateOption(BuyPotion, "TH_ALICE-LOUIS_SHOP.pages.INITIAL.options.BUY_POTION"),CreateOption(BuyHeal, "TH_ALICE-LOUIS_SHOP.pages.INITIAL.options.BUY_HEAL"),buyRelic});
    }
    
    private async Task BuyRelic()
    {
         await PlayerCmd.LoseGold(300, Owner!, GoldLossType.Stolen);
        RelicModel relic = RelicFactory.PullNextRelicFromFront(base.Owner, RelicRarity.Common).ToMutable();
		await RelicCmd.Obtain(relic, base.Owner);
        RelicModel relic2 = RelicFactory.PullNextRelicFromFront(base.Owner, RelicRarity.Uncommon).ToMutable();
		await RelicCmd.Obtain(relic2, base.Owner);
        RelicModel relic3 = RelicFactory.PullNextRelicFromFront(base.Owner, RelicRarity.Rare).ToMutable();
		await RelicCmd.Obtain(relic3, base.Owner);
        SetEventFinished(PageDescription("BUY_RELIC")); 
    }
    private async Task BuyPotion()
    {
        await PlayerCmd.LoseGold(180, Owner!, GoldLossType.Stolen);
        SetEventFinished(PageDescription("BUY_POTION"));
        List<Reward> list = new List<Reward>();
		list.Add(new PotionReward(base.Owner));
		list.Add(new PotionReward(base.Owner));
		list.Add(new PotionReward(base.Owner));
		await RewardsCmd.OfferCustom(base.Owner, list);
    }

     private async Task BuyHeal()
    {
         await PlayerCmd.LoseGold(100, Owner!, GoldLossType.Stolen);
       await CreatureCmd.Heal(Owner.Creature,Owner.Creature.MaxHp,false);
        SetEventFinished(PageDescription("BUY_HEAL"));
    }
}

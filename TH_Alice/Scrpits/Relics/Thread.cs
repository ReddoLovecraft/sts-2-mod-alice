using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using TH_Alice.Scrpits.Cards;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Dolls;
using TH_Alice.Scrpits.Main;
using TH_Alice.Scrpits.Powers;
using TH_Alice.TH_Alice.Scrpits.Main;

[Pool(typeof(AliceRelicPool))]
public class Thread : CustomRelicModel
{
    private readonly List<(Creature creature, int damage)> _pendingDollDamages = new List<(Creature, int)>();
	public override RelicRarity Rarity => RelicRarity.Starter;
	public override string PackedIconPath => $"res://ArtWorks/Relics/{Id.Entry}.png";
	protected override string PackedIconOutlinePath => $"res://ArtWorks/Relics/Outlines/{Id.Entry}.png";
	protected override string BigIconPath => $"res://ArtWorks/Relics/{Id.Entry}.png";
	static string text = StringHelper.Slugify("Doll");
	static  LocString locString = ToolBox.L10NStatic(text + ".title");
	static  LocString locString2 = ToolBox.L10NStatic(text + ".description");

	protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
	{
		new HoverTip(locString,locString2),
		HoverTipFactory.FromCard<Manipulate>()
	});
	public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
	{
		//塔2回合开始不再有怪物和玩家的区别，需要手动判断
		if (side != base.Owner.Creature.Side)
		{
			return;
		}
		Flash();
		await CardPileCmd.AddGeneratedCardToCombat(combatState.CreateCard<Manipulate>(Owner), PileType.Hand, true);
	}
    public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        _pendingDollDamages.Clear();
        if (target != base.Owner.Creature || Owner.Creature.HasPower<AliceUnreadPower>())
        {
            return amount;
        }
        List<Creature> dolls = target.Pets
            .Where(p => p.IsAlive && p.Monster is AliceDollMonsterModel doll && doll.ParticipatesInDamageShare)
            .ToList();

        int damage = (int)amount;
        int count = dolls.Count;
        if (count == 0 || damage <= 0)
        {
            return amount;
        }

        int perDollDamage = damage / count;
        if (perDollDamage <= 0)
        {
            return 0;
        }

        bool wouldKillAll = dolls.All(d => d.CurrentHp + d.Block <= perDollDamage);
        if (wouldKillAll)
        {
            return amount;
        }
        foreach (Creature doll in dolls)
        {
            _pendingDollDamages.Add((doll, perDollDamage));
        }
        return 0;

    }
    public override Task AfterModifyingHpLostAfterOsty()
    {
        Flash();
        return RunPendingAsync();
    }
    private bool _isProcessing;
    private async Task RunPendingAsync()
    {
        if (_isProcessing) return;
        _isProcessing = true;
        try
        {
            while (_pendingDollDamages.Count > 0)
            {
                var damages = _pendingDollDamages.ToArray();
                _pendingDollDamages.Clear();
                foreach (var item in damages)
                {
                    Creature dollCreature = item.creature;
                    int dmg = item.damage;
                    if (dollCreature == null || dollCreature.IsDead || dmg <= 0)
                    {
                        continue;
                    }
                    await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), dollCreature, dmg, ValueProp.Move, null, null);
                }
            }
        }
        finally
        {
            _isProcessing = false;
        }
    }
}

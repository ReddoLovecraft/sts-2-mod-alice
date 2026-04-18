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
public class BloodThread : CustomRelicModel
{
    private readonly List<(Creature creature, int damage)> _pendingDollDamages = new List<(Creature, int)>();
	public override RelicRarity Rarity => RelicRarity.Ancient;
	public override string PackedIconPath => $"res://ArtWorks/Relics/{Id.Entry}.png";
	protected override string PackedIconOutlinePath => $"res://ArtWorks/Relics/Outlines/{Id.Entry}.png";
	protected override string BigIconPath => $"res://ArtWorks/Relics/{Id.Entry}.png";
	//效果：你的回合开始时，恢复所有人偶3点生命并将一张操控加入手牌。
	//操控：0费消耗，敲完抽1。 指定一个目标，本回合内所有人偶的攻击将优先指定该目标
	//人偶：类似小手和充能球，当你受到未被格挡的伤害时，伤害由所有人偶均摊（向下取整），回合结束时，根据人偶的类型触发不同的效果(看看代码考虑是用奥斯提还是怪物行动意图)
	//暂定人偶：4种。
	//上海人偶：一般攻击人偶，12血，回合结束时对随机敌人造成8点伤害。
	//歌莉娅人偶：防御型人偶，比较肉。回合结束时，所有友方角色获得5点格挡。20血。
	//蓬莱人偶：脆弱buff人偶，8血，回合结束时对随机敌人施加2层虚弱/2层易伤（轮替），并使所有拥有负面状态的敌人失去5*负面状态层数点生命。
	//大江户人偶：自爆型人偶，自杀式袭击。10血，死亡时对所有角色造成等同于自身最大生命的伤害，如果没被击杀每回合+5血上限。
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
        foreach (Creature pet in Owner.Creature.Pets)
        {
            if (pet.IsAlive && pet.Monster is AliceDollMonsterModel doll && doll.ParticipatesInDamageShare)
            {
                await CreatureCmd.GainMaxHp(pet, 5);
            }
        }
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

        int overflowToAlice = 0;
        foreach (Creature doll in dolls)
        {
            int effective = doll.CurrentHp + doll.Block;
            if (effective < perDollDamage)
            {
                overflowToAlice += perDollDamage - effective;
            }
            _pendingDollDamages.Add((doll, perDollDamage));
        }
        return overflowToAlice;

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

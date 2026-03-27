using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
public class CurseHangPengLai : AliceCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override bool ShouldGlowGoldInternal => Owner.Creature.HasPower<PengLaiPower>();
    protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new DynamicVar("Power", 3)
     ];
    static string text = StringHelper.Slugify("Doll");
    static LocString locString = ToolBox.L10NStatic(text + ".title");
    static LocString locString2 = ToolBox.L10NStatic(text + ".description");
    static string text2 = StringHelper.Slugify("Penglai");
    static LocString locString3 = ToolBox.L10NStatic(text2 + ".title");
    static LocString locString4 = ToolBox.L10NStatic(text2 + ".description");
    static string text3 = StringHelper.Slugify("Recycle");
    static LocString locString5 = ToolBox.L10NStatic(text3 + ".title");
    static LocString locString6 = ToolBox.L10NStatic(text3 + ".description");
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[5]
  {
        new HoverTip(locString5,locString6),
        new HoverTip(locString3,locString4),
        new HoverTip(locString,locString2),
           HoverTipFactory.FromPower<VulnerablePower>(),
         HoverTipFactory.FromPower<WeakPower>()
  });
    public CurseHangPengLai() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        VfxCmd.PlayOnCreatureCenter(base.Owner.Creature, "vfx/vfx_flying_slash");
        int amount = base.DynamicVars["Power"].IntValue;
        foreach (Creature enemy in base.CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<WeakPower>(enemy, amount, base.Owner.Creature, this);
            await PowerCmd.Apply<VulnerablePower>(enemy, amount, base.Owner.Creature, this);
        }
        if (ShouldGlowGoldInternal) 
        {
            int amt = 0;
                foreach (PowerModel pm in Owner.Creature.Powers)
                {
                    if (pm is PengLaiPower plm)
                    {
                        amt+=plm.Amount;
                    break;
                    }
                }
            await ToolBox.RecycleDolls(base.Owner.Creature, 1, PengLaiFirst: true);
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature.CombatState.HittableEnemies, amt , ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature, null);
        }
    }
	protected override void OnUpgrade()
	{
        base.DynamicVars["Power"].UpgradeValueBy(1);
        base.EnergyCost.UpgradeBy(-1);
    }
}

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
public class TripWire : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
         new DynamicVar("Power", 8),
        new CalculationBaseVar(0),
        new ExtraDamageVar(0),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) =>ToolBox.GetDollCount(card.Owner.Creature))
        ];
    
    static string text = StringHelper.Slugify("Doll");
    static LocString locString = ToolBox.L10NStatic(text + ".title");
    static LocString locString2 = ToolBox.L10NStatic(text + ".description");
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
 {
        HoverTipFactory.FromPower<StrengthPower>(),
        new HoverTip(locString,locString2)
 });
    public TripWire() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
	{
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        SfxCmd.Play(AliceModInit.ToModSfxPath("ArtWorks/SFX/line.wav"));
        await PowerCmd.Apply<MgrPower>(cardPlay.Target, 1, base.Owner.Creature, this);
        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage).FromCard(this).Targeting(cardPlay.Target)
            .WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/giant_louse/giant_louse_attack_web")
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
        await PowerCmd.Apply<PiercingWailPower>(cardPlay.Target, base.DynamicVars["Power"].BaseValue, base.Owner.Creature, this);
    }
    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}

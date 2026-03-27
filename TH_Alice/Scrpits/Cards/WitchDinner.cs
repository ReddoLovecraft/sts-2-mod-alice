using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(AliceCardPool))]
public class WitchDinner : AliceCardModel
{
    public override bool CanBeGeneratedInCombat => false;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
		new DamageVar(12m, ValueProp.Move),
        new MaxHpVar(3m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
 {
     HoverTipFactory.Static(StaticHoverTip.Fatal)
 });
    public WitchDinner() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{

        foreach(Creature mos in Owner.Creature.CombatState.HittableEnemies) 
        {
            if(mos.IsDead) continue;
            ArgumentNullException.ThrowIfNull(mos, "mos");
            bool shouldTriggerFatal = mos.Powers.All((PowerModel p) => p.ShouldOwnerDeathTriggerFatal());
            AttackCommand attackCommand = await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(mos)
                .WithHitFx("vfx/vfx_bite", null, "blunt_attack.mp3")
                .Execute(choiceContext);
            if (shouldTriggerFatal && attackCommand.Results.Any((DamageResult r) => r.WasTargetKilled))
            {
                await CreatureCmd.GainMaxHp(base.Owner.Creature, base.DynamicVars.MaxHp.IntValue);
            }
            await CreatureCmd.Heal(base.Owner.Creature, attackCommand.Results.Sum((DamageResult r) => r.TotalDamage + r.OverkillDamage));
        }
    }
	protected override void OnUpgrade()
	{
        base.DynamicVars.MaxHp.UpgradeValueBy(2m);
    }
}

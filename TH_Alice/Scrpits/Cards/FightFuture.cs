using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(ColorlessCardPool))]
public class FightFuture : AliceCardModel
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(2)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
{
        base.EnergyHoverTip
});
    public FightFuture() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
         await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature)
                                           where c != null && c.IsAlive && c.IsPlayer
                                           select c;
        foreach (Creature item in enumerable)
        {
            await PowerCmd.Apply<EnergyNextTurnPower>(item, base.DynamicVars.Energy.IntValue, base.Owner.Creature, this);
        }
    }
	protected override void OnUpgrade()
	{
		DynamicVars.Energy.UpgradeValueBy(1); 
	}
}

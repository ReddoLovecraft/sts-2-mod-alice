using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Alice.Scrpits.Main;
using TH_Alice.Scrpits.Character;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(AliceCardPool))]
public class Discussion : AliceCardModel
{
	public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
	  protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
{
         HoverTipFactory.FromPower<VulnerablePower>()
});
	public Discussion() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature)
			where c != null && c.IsAlive && c.IsPlayer
			select c;
		foreach (Creature item in enumerable)
		{
			await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, item.Player);
		}
		 foreach (Creature enemy in base.CombatState.HittableEnemies)
 {
     await PowerCmd.Apply<WeakPower>(enemy, 2, base.Owner.Creature, this);
     await PowerCmd.Apply<VulnerablePower>(enemy, 2, base.Owner.Creature, this);
 }
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Cards.UpgradeValueBy(1);
	}
}

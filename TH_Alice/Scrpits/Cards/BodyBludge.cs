using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(AliceCardPool))]
public class BodyBludge : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7, ValueProp.Move), new DynamicVar("Power", 3)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
{
         HoverTipFactory.FromPower<StrengthPower>()
});
    public BodyBludge() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(DynamicVars.Damage.BaseValue) .FromCard(this) .Targeting(cardPlay.Target).Execute(choiceContext);
		if(cardPlay.Target.IsAlive)
		{
			await PowerCmd.Apply<StrengthPower>(cardPlay.Target, -base.DynamicVars["Power"].IntValue, Owner.Creature, this);
			await PowerCmd.Apply<StrengthPower>(Owner.Creature, base.DynamicVars["Power"].IntValue, Owner.Creature, this);
        }
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(2);
        base.DynamicVars["Power"].UpgradeValueBy(1);
    }
}

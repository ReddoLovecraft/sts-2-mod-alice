using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Alice.Scrpits.Main;
using TH_Alice.Scrpits.Character;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(AliceCardPool))]
public class ManaRunOut : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(3)];
	 protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
        base.EnergyHoverTip
  });
	public ManaRunOut() : base(1, CardType.Skill, CardRarity.Common, TargetType.None)
	{

	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue,Owner);
		await PowerCmd.Apply<WasteAwayPower>(Owner.Creature,1,Owner.Creature,this);
	}
	protected override void OnUpgrade()
	{
		base.EnergyCost.UpgradeBy(-1);
	}
}

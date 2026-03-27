using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Main;
using TH_Alice.Scrpits.Powers;
using TH_Alice.TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(AliceCardPool))]
public class EnglandRose : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
         new DynamicVar("Power", 4)
     ];
   
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
          HoverTipFactory.FromPower<ThornsPower>(),
  });
    public EnglandRose() : base(1, CardType.Power ,CardRarity.Rare, TargetType.Self)
	{
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
      await PowerCmd.Apply<ThornsPower>(Owner.Creature, base.DynamicVars["Power"].BaseValue, base.Owner.Creature, this);
      await PowerCmd.Apply<RosePower>(Owner.Creature,1, base.Owner.Creature, this);
    }
	protected override void OnUpgrade()
	{
        base.DynamicVars["Power"].UpgradeValueBy(2);
    }
}

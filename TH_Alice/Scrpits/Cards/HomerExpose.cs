using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
public class HomerExpose : AliceCardModel
{
   public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
      new DynamicVar("Power", 2),
      new BlockVar(10m, ValueProp.Move)
     ];
   
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
         HoverTipFactory.FromPower<VulnerablePower>()
  });
    public HomerExpose() : base(1, CardType.Skill ,CardRarity.Common, TargetType.Self)
	{
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        await PowerCmd.Apply<VulnerablePower>(Owner.Creature, base.DynamicVars["Power"].BaseValue, base.Owner.Creature, this);
           await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
    }
	protected override void OnUpgrade()
	{
        base.DynamicVars["Power"].UpgradeValueBy(-1);
         base.DynamicVars.Block.UpgradeValueBy(2);
    }
}

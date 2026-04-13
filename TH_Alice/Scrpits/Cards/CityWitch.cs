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
public class CityWitch : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new DynamicVar("Power", 3),
        new EnergyVar(1)
     ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
  {
        base.EnergyHoverTip,
        HoverTipFactory.FromPower<FrailPower>()
  });
    public CityWitch() : base(1, CardType.Power ,CardRarity.Uncommon, TargetType.Self)
	{
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<FrailPower>(Owner.Creature, base.DynamicVars["Power"].IntValue, base.Owner.Creature, this);
        await PowerCmd.Apply<CityWitchPower>(Owner.Creature, 1, base.Owner.Creature, this);
    }
    protected override void OnUpgrade()
	{
        base.DynamicVars["Power"].UpgradeValueBy(-1);
        AddKeyword(CardKeyword.Innate);
    }
}

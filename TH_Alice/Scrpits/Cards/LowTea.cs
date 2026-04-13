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
public class LowTea : AliceCardModel
{
    public override bool CanBeGeneratedInCombat => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        
     ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
   
    public LowTea() : base(2, CardType.Power ,CardRarity.Rare, TargetType.Self)
	{
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<LowTeaPower>(Owner.Creature, 1, base.Owner.Creature, this);
        PlayerCmd.EndTurn(base.Owner, canBackOut: false);
    }
	protected override void OnUpgrade()
	{
        base.EnergyCost.UpgradeBy(-1);
    }
}

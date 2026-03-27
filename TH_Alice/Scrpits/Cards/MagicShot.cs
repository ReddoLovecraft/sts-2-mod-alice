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
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Main;
using TH_Alice.Scrpits.Powers;
using TH_Alice.TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(AliceCardPool))]
public class MagicShot : AliceCardModel
{
    public override int MaxUpgradeLevel => 8;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
       new DamageVar(3,ValueProp.Move),
       new CardsVar(3)
     ];
    public MagicShot() : base(1, CardType.Attack ,CardRarity.Common, TargetType.AnyEnemy)
	{
	}
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
       Rng rng = Owner.Creature.Player.RunState.Rng.CombatCardGeneration;
       int randomNumber = rng.NextInt(1, 5)+1;
       await DamageCmd.Attack(randomNumber).FromCard(this).Targeting(cardPlay.Target).WithHitCount(base.DynamicVars.Cards.IntValue).Execute(choiceContext);
    }
	protected override void OnUpgrade()
	{
        base.DynamicVars.Cards.UpgradeValueBy(1);
    }
}

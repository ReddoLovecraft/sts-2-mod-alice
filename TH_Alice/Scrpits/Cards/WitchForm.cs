using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Main;
using TH_Alice.Scrpits.Powers;
using TH_Alice.TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(AliceCardPool))]
public class WitchForm : AliceCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [];
    public override bool CanBeGeneratedInCombat => false;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
  {
      HoverTipFactory.FromCard<GodGun>(),
        HoverTipFactory.FromCard<GodReuse>()
  });
    public WitchForm() : base(3, CardType.Power ,CardRarity.Ancient, TargetType.Self)
	{
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        await PowerCmd.Apply<WitchFormPower>(Owner.Creature,1, base.Owner.Creature, this);
       
    }
	protected override void OnUpgrade()
	{
        base.EnergyCost.UpgradeBy(-1);
        base.AddKeyword(CardKeyword.Innate);
    }
}

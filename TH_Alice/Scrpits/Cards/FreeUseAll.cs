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
public class FreeUseAll : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        
     ];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public FreeUseAll() : base(2, CardType.Skill ,CardRarity.Uncommon, TargetType.None)
	{
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        SfxCmd.Play(AliceModInit.ToModSfxPath("ArtWorks/SFX/cast.wav"));
        await PowerCmd.Apply<FreeSkillPower>(Owner.Creature, 1, base.Owner.Creature, this);
        await PowerCmd.Apply<FreePowerPower>(Owner.Creature, 1, base.Owner.Creature, this);
        await PowerCmd.Apply<FreeAttackPower>(Owner.Creature, 1, base.Owner.Creature, this);
    }
	protected override void OnUpgrade()
	{
        RemoveKeyword(CardKeyword.Exhaust);
    }
}

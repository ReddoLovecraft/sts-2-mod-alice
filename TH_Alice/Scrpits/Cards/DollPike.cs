using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Dolls;
using TH_Alice.Scrpits.Main;
using TH_Alice.Scrpits.Powers;
using TH_Alice.TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(AliceCardPool))]
public class DollPike : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new RepeatVar(3),
        new CalculationBaseVar(3m),
        new ExtraDamageVar(2m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) =>ToolBox.GetDollCount(card.Owner.Creature))
    ];
    static string text = StringHelper.Slugify("Doll");
    static LocString locString = ToolBox.L10NStatic(text + ".title");
    static LocString locString2 = ToolBox.L10NStatic(text + ".description");
  
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
        new HoverTip(locString,locString2)
  });
    public DollPike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        SfxCmd.Play(AliceModInit.ToModSfxPath("ArtWorks/SFX/muti.wav"));
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage).WithHitCount((int)base.DynamicVars.Repeat.BaseValue).FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
      

    }
    protected override void OnUpgrade()
    {
        base.DynamicVars.Repeat.UpgradeValueBy(1);
        base.DynamicVars.ExtraDamage.UpgradeValueBy(1m);
    }
}

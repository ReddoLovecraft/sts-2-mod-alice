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
public class SeekWire : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new DamageVar(6, ValueProp.Unblockable|ValueProp.Unpowered)
     ];
    static string text = StringHelper.Slugify("Doll");
    static LocString locString = ToolBox.L10NStatic(text + ".title");
    static LocString locString2 = ToolBox.L10NStatic(text + ".description");
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
       
        new HoverTip(locString,locString2)
  });
    public SeekWire() : base(1, CardType.Attack ,CardRarity.Common, TargetType.AllEnemies)
	{
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        SfxCmd.Play(AliceModInit.ToModSfxPath("ArtWorks/SFX/line.wav"));
        int cnt = ToolBox.GetDollCount(Owner.Creature);
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(cnt+1).FromCard(this)
             .TargetingRandomOpponents(base.CombatState)
             .WithHitFx("vfx/vfx_attack_slash")
             .Execute(choiceContext);
    }
	protected override void OnUpgrade()
	{
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}

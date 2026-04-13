using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
public class DevilryLightRay : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
     [
        new DamageVar(8, ValueProp.Unblockable|ValueProp.Unpowered)
     ];
  
   
    public DevilryLightRay() : base(1, CardType.Attack ,CardRarity.Uncommon, TargetType.AllEnemies)
	{
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        SfxCmd.Play(AliceModInit.ToModSfxPath("ArtWorks/SFX/line.wav"));
        foreach(Creature mos in Owner.Creature.CombatState.HittableEnemies) 
        {
            if (mos.IsAlive) 
            {
                int muti = IsUpgraded ? ToolBox.GetDebuffTotalCount(mos) : ToolBox.GetDebuffKind(mos);
                muti++;
                if (muti<=0)muti = 1;
                VfxCmd.PlayOnCreatureCenter(mos, "vfx/vfx_bloody_impact");
                await CreatureCmd.Damage(choiceContext, mos, new DamageVar(base.DynamicVars.Damage.BaseValue*muti, ValueProp.Unblockable|ValueProp.Unpowered), this);
            }
        }
    }
	protected override void OnUpgrade()
	{
        base.DynamicVars.Damage.UpgradeValueBy(3);
    }
}

using BaseLib.Utils;
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
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Main;
using TH_Alice.TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(AliceCardPool))]
public sealed class ArtfulSacrifice : AliceCardModel
{
  
    protected override IEnumerable<DynamicVar> CanonicalVars => (new DynamicVar[3]
{
        new HpLossVar(4m),
		new EnergyVar(1),
		new CardsVar(2)
});
    static string text = StringHelper.Slugify("Doll");
    static LocString locString = ToolBox.L10NStatic(text + ".title");
    static LocString locString2 = ToolBox.L10NStatic(text + ".description");
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
  {
        base.EnergyHoverTip,
        new HoverTip(locString,locString2)
  });
    public ArtfulSacrifice() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }
    protected override bool IsPlayable => ToolBox.GetDollCount(Owner.Creature) > 0;
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        if (ToolBox.GetDollCount(Owner.Creature)>0)
        {
     
            foreach(PowerModel pm in Owner.Creature.Powers) 
            {
                if(pm is AlicePowerModel apm&&apm.IsDollPower)
                {
                    await apm.APM_Decline(base.DynamicVars.HpLoss.IntValue);
                    break;
                }
            }
            await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, base.Owner);
            await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
        }
    }
    
    protected override void OnUpgrade()
    {
        base.DynamicVars.Energy.UpgradeValueBy(1);
    }
}

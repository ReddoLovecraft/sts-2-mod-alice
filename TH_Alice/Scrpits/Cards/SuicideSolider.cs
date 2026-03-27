using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using System.Collections.Generic;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Main;
using TH_Alice.TH_Alice.Scrpits.Main;


namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(AliceCardPool))]
public class SuicideSolider : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    static string text = StringHelper.Slugify("Doll");
    static LocString locString = ToolBox.L10NStatic(text + ".title");
    static LocString locString2 = ToolBox.L10NStatic(text + ".description");
    static string text2 = StringHelper.Slugify("Recycle");
    static LocString locString3 = ToolBox.L10NStatic(text2 + ".title");
    static LocString locString4 = ToolBox.L10NStatic(text2 + ".description");
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
  {
        new HoverTip(locString,locString2),
        new HoverTip(locString3,locString4)
  });
    public SuicideSolider() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
    }
    protected override bool IsPlayable => ToolBox.GetDollCount(Owner.Creature) > 0;
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        List<AlicePowerModel> dolls = new List<AlicePowerModel>();
        foreach (PowerModel pm in Owner.Creature.Powers)
        {
            if (pm is AlicePowerModel apm && apm.IsDollPower)
            {
                dolls.Add(apm);
            }
        }
        foreach (AlicePowerModel apm in dolls)
        {
            int cnt = apm.Amount * (int)base.DynamicVars.Cards.BaseValue;
            await DamageCmd.Attack(cnt).WithHitCount(1).FromCard(this)
                .TargetingRandomOpponents(base.CombatState)
                .WithHitVfxNode((Creature t) => NScratchVfx.Create(t, goingRight: true))
                .Execute(choiceContext);
        }
        await ToolBox.RecycleDolls(Owner.Creature, ToolBox.GetDollCount(Owner.Creature));
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }

}

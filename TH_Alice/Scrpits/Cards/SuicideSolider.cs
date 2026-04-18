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
using System.Linq;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Dolls;
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
        var dolls = Owner.Creature.Pets.Where(p => p.IsAlive && p.Monster is AliceDollMonsterModel).ToList();
        foreach (Creature dollCreature in dolls)
        {
            int cnt = dollCreature.CurrentHp * (int)base.DynamicVars.Cards.BaseValue;
            await DamageCmd.Attack(cnt).WithHitCount(1).FromCard(this)
                .TargetingRandomOpponents(base.CombatState)
                .WithHitVfxNode((Creature t) => NScratchVfx.Create(t, goingRight: true))
                .Execute(choiceContext);
            SfxCmd.Play(AliceModInit.ToModSfxPath("ArtWorks/SFX/bomb.wav"));
        }
        await ToolBox.RecycleDolls(Owner.Creature, dolls.Count);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }

}

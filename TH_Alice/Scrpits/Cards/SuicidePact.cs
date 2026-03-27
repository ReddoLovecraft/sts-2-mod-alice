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
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Main;
using TH_Alice.Scrpits.Powers;
using TH_Alice.TH_Alice.Scrpits.Main;
using static MegaCrit.Sts2.Core.Models.Monsters.KnowledgeDemon;

namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(AliceCardPool))]
public class SuicidePact : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];
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
    protected override bool IsPlayable => ToolBox.GetDollCount(Owner.Creature) > 0;
    public SuicidePact() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        foreach (PowerModel pm in Owner.Creature.Powers)
        {
            if (pm is AlicePowerModel apm && apm.IsDollPower)
            {
                apm.AddDamage(DynamicVars.Cards.BaseValue);
            }
        }
        await ToolBox.RecycleDolls(Owner.Creature, 1);
    }
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(3);
    }
}

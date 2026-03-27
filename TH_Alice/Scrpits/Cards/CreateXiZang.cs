using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Main;
using TH_Alice.Scrpits.Powers;
using TH_Alice.TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Cards
{
    [Pool(typeof(ColorlessCardPool))]
    public sealed class CreateXiZang : AliceCardModel
    {
        public override bool CanBeGeneratedInCombat => true;

        public override int MaxUpgradeLevel => 0;
        static string text = StringHelper.Slugify("Doll");
        static string text2 = StringHelper.Slugify("Xizang");
        static LocString locString = ToolBox.L10NStatic(text + ".title");
        static LocString locString2 = ToolBox.L10NStatic(text + ".description");
        static LocString locString3 = ToolBox.L10NStatic(text2 + ".title");
        static LocString locString4 = ToolBox.L10NStatic(text2 + ".description");
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
{
       new HoverTip(locString3,locString4),
        new HoverTip(locString,locString2)
});
        public CreateXiZang()
            : base(-1, CardType.Quest, CardRarity.Quest, TargetType.None)
        {
        }

        public override async Task OnChosen()
        {
            await ToolBox.MakeDoll<XiZangPower>(Owner.Creature); 
        }
    }

}

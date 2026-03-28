using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH_Alice.Scrpits.Cards;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Main;
using TH_Alice.Scrpits.Powers;
using TH_Alice.TH_Alice.Scrpits.Main;

    [Pool(typeof(AliceRelicPool))]
    public class Silk : CustomRelicModel
    {
        protected override IEnumerable<DynamicVar> CanonicalVars => [(new EnergyVar(1))];
        public override RelicRarity Rarity => RelicRarity.Rare;
        public override string PackedIconPath => $"res://ArtWorks/Relics/{Id.Entry}.png";
        protected override string PackedIconOutlinePath => $"res://ArtWorks/Relics/Outlines/{Id.Entry}.png";
        protected override string BigIconPath => $"res://ArtWorks/Relics/{Id.Entry}.png";
         static string text = StringHelper.Slugify("Doll");
        static LocString locString = ToolBox.L10NStatic(text + ".title");
        static LocString locString2 = ToolBox.L10NStatic(text + ".description");
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
        {
       HoverTipFactory.ForEnergy(this),
       new HoverTip(locString,locString2)
        });
    }



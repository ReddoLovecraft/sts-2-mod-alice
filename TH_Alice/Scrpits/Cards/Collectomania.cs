using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
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
    [Pool(typeof(CurseCardPool))]
    public sealed class Collectomania : AliceCardModel
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Eternal, CardKeyword.Unplayable];
        public override bool CanBeGeneratedInCombat => false;
        public override bool HasTurnEndInHandEffect => true;
        public override int MaxUpgradeLevel => 0;
        public Collectomania()
            : base(-1, CardType.Curse, CardRarity.Curse, TargetType.None)
        {
        }
        public override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
        {
            int hpLoss = (Owner.Creature.Player.RunState.TotalFloor - Owner.Creature.Player.Relics.Count);
            if (hpLoss > 0) 
                await CreatureCmd.Damage(choiceContext, base.Owner.Creature, hpLoss,  ValueProp.Unpowered | ValueProp.Move, this);
            else
                await CreatureCmd.Heal(base.Owner.Creature, -hpLoss);
        }
    }

}

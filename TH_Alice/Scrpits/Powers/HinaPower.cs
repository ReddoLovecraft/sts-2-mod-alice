using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.ValueProps;

using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Powers
{
    public sealed class HinaPower:AlicePowerModel
    {
       
        public override PowerType Type => PowerType.Buff;
        public override bool IsDollPower => true;
        public override bool IsInstanced => true;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://ArtWorks/Powers/HP32.png";
        public override string? CustomBigIconPath => "res://ArtWorks/Powers/HP64.png";
       
        public HinaPower() {
            
        }
        public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side != base.Owner.Side)
            {
                return;
            }
            await DollAction(choiceContext);
            await base.BeforeTurnEnd(choiceContext, side);
        }
        public override async Task DollAction(PlayerChoiceContext choiceContext)
        {
            await PowerCmd.Apply<ArtifactPower>(Owner, base.DynamicVars.Damage.BaseValue, Owner, null);
        }
        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner.Player)
            {
                return;
            }
            await base.AfterPlayerTurnStart(choiceContext, player);
        }
        
    }
}

using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Powers
{
    public sealed class PengLaiPower:AlicePowerModel
    {
        
        public override PowerType Type => PowerType.Buff;
        public override bool IsDollPower => true;
        public override bool IsInstanced => true;
        public PlayerChoiceContext ChoiceContextEndTurn;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://ArtWorks/Powers/PLP32.png";
        public override string? CustomBigIconPath => "res://ArtWorks/Powers/PLP64.png";
        
        public PengLaiPower() {
            
        }
        public async override Task DollAction(PlayerChoiceContext choiceContext)
        {
            await PowerCmd.ModifyAmount(this, base.DynamicVars.Damage.BaseValue, null, null);
        }
        public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side != base.Owner.Side)
            {
                return;
            }
            ChoiceContextEndTurn = choiceContext;
           await base.BeforeTurnEnd(choiceContext, side);
        }
        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner.Player)
            {
                return;
            }
            await DollAction(choiceContext);
            await base.AfterPlayerTurnStart(choiceContext, player);
        }
       
    }
}

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
    public sealed class LondonPower:AlicePowerModel
    {
       
        public override PowerType Type => PowerType.Buff;
        public override bool IsDollPower => true;
        public override bool IsInstanced => true;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://ArtWorks/Powers/LP32.png";
        public override string? CustomBigIconPath => "res://ArtWorks/Powers/LP64.png";
       
        public LondonPower() {
            
        }
        public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side != base.Owner.Side)
            {
                return;
            }
            await base.BeforeTurnEnd(choiceContext, side);
        }
        public override async Task DollAction(PlayerChoiceContext choiceContext)
        {
            foreach (Creature monster in Owner.CombatState.Enemies)
            {
                if (monster.IsAlive)
                {
                    await PowerCmd.Apply<PoisonPower>(monster, base.DynamicVars.Damage.BaseValue, Owner, null);
                }

            }
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

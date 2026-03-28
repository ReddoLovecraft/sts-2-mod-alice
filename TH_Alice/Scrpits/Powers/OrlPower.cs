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
    public sealed class OrlPower:AlicePowerModel
    {
       
        public override PowerType Type => PowerType.Buff;
        public override bool IsDollPower => true;
        public override bool IsInstanced => true;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://ArtWorks/Powers/OP32.png";
        public override string? CustomBigIconPath => "res://ArtWorks/Powers/OP64.png";

        public OrlPower() {
            
        }
        public async override Task DollAction(PlayerChoiceContext choiceContext, bool Repeatable = true)
        {
            await CreatureCmd.GainBlock(Owner, base.DynamicVars.Damage.BaseValue, ValueProp.Unpowered, null);
            List<Creature> target = new List<Creature>();
            foreach (Creature monster in base.CombatState.HittableEnemies)
            {
                if (monster.HasPower<MgrPower>())
                {
                    target.Add(monster);
                    break;
                }
            }
            if (target.Count == 0)
                target.Add(Owner.Player.RunState.Rng.CombatTargets.NextItem(base.CombatState.HittableEnemies));
            if (target != null && target[0] != null && target[0].IsAlive)
                await CreatureCmd.Damage(choiceContext, target, Owner.Block, ValueProp.Unpowered, base.Owner);
            if (Owner.Player.GetRelic<Silk>() != null)
            {
                await PowerCmd.Apply<EnergyNextTurnPower>(Owner, 1, Owner, null);
            }
            if (Owner != null && Owner.HasPower<LubePower>() && Repeatable)
            {
                await DollAction(choiceContext, false);
            }
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

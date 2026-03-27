
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Powers
{
    public sealed class DeathWitchPower : AlicePowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://ArtWorks/Powers/DWP32.png";
        public override string? CustomBigIconPath => "res://ArtWorks/Powers/DWP64.png";
        private bool HasAttacked=false;
        public DeathWitchPower() { }
        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner.Player)
            {
                return;
            }
            HasAttacked=false;
        }
        public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (!props.IsPoweredAttack_())
            {
                return 1m;
            }
            if (cardSource == null)
            {
                return 1m;
            }
            if (dealer != Owner)
            {
                return 1m;
            }
            return Amount;
        }

        public async override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if(cardPlay.Card.Type==CardType.Attack)
            {
               HasAttacked=true;
            }
            await base.AfterCardPlayed(context, cardPlay);
        }
        public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side != base.Owner.Side)
            {
                return;
            }
            if (!HasAttacked)
            {
                await CreatureCmd.Damage(choiceContext, Owner, 6, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, Owner);
            }
        }
    }
    

}
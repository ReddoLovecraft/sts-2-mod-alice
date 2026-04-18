
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TH_Alice.Scrpits.Dolls;
using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Powers
{
    public sealed class FlowerPower : AlicePowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://ArtWorks/Powers/FP232.png";
        public override string? CustomBigIconPath => "res://ArtWorks/Powers/FP264.png";

        public FlowerPower() { }
        public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side != base.Owner.Side)
            {
                return;
            }
            foreach (Creature pet in Owner.Pets)
            {
                if (pet.IsAlive && pet.Monster is AliceDollMonsterModel)
                {
                    await CreatureCmd.GainMaxHp(pet, Amount);
                }
            }
        }
    }
    

}

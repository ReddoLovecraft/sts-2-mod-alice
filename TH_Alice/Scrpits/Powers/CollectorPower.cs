
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using TH_Alice.Scrpits.Main;
using TH_Alice.Scrpits.Relics;

namespace TH_Alice.Scrpits.Powers
{
    public sealed class CollectorPower : AlicePowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override bool IsInstanced => true;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://ArtWorks/Powers/CP232.png";
        public override string? CustomBigIconPath => "res://ArtWorks/Powers/CP264.png";

        public CollectorPower() { }
    
        public override Task AfterCombatEnd(CombatRoom room)
        {
            room.AddExtraReward(base.Owner.Player, new RelicReward(Owner.Player));
            return Task.CompletedTask;
        }
    }

    

}
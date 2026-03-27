
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using TH_Alice.Scrpits.Main;
using TH_Alice.TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Powers
{
    public sealed class FutureDollPower : AlicePowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://ArtWorks/Powers/FDP32.png";
        public override string? CustomBigIconPath => "res://ArtWorks/Powers/FDP64.png";

        public FutureDollPower() { }
        public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side != CombatSide.Player)
            {
                return;
            }
           if (Owner.Player.PlayerCombatState.Energy > 0) 
            { 
            
                for(int i = 0; i <Amount; i++) 
                {
                    await ToolBox.MakeRandomDoll(Owner);
                }
            }
        }
    }
    

}
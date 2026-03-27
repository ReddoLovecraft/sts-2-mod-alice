
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Powers
{
    public sealed class DollAutoPower : AlicePowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://ArtWorks/Powers/DAP32.png";
        public override string? CustomBigIconPath => "res://ArtWorks/Powers/DAP64.png";
        public DollAutoPower() { }
        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
                Flash();
            List<AlicePowerModel> apms=new List<AlicePowerModel>();
              foreach(PowerModel pm in cardPlay.Card.Owner.Creature.Powers) 
            {
                if(pm is AlicePowerModel apm && apm.IsDollPower) 
                {
                    apms.Add(apm);
                }
            }
            for (int i = apms.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < Amount; j++)
                    await apms[i].DollAction(context);
                apms.RemoveAt(i);
            }
          
        }
        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            await PowerCmd.Remove(this);
        }
    }
    

}

using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Random;
using TH_Alice.Scrpits.Main;
using TH_Alice.TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Powers
{
    public sealed class DollDancePower : AlicePowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://ArtWorks/Powers/DDP32.png";
        public override string? CustomBigIconPath => "res://ArtWorks/Powers/DDP64.png";
        public DollDancePower() { }
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
            if(apms.Count>0)
            for(int i=0;i<Amount;i++)
            {
            Rng rng = Owner.Player.RunState.Rng.CombatCardGeneration;
            int randomNumber = rng.NextInt(0, apms.Count);
            if(apms[randomNumber]!=null)
            await apms[randomNumber].DollAction(context);
            }
            else for(int i=0;i<Amount;i++)
            {
                await ToolBox.MakeRandomDoll(Owner);
            }
        }
    }
    

}
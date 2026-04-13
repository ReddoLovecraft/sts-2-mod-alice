
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using TH_Alice.Scrpits.Cards;
using TH_Alice.Scrpits.Main;
using TH_Alice.TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Powers
{
    public sealed class WorkshopPower : AlicePowerModel
    {
        public override PowerType Type => PowerType.Buff;
        private int cnt = 0;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://ArtWorks/Powers/WP32.png";
        public override string? CustomBigIconPath => "res://ArtWorks/Powers/WP64.png";

        public WorkshopPower() { }
        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
             if(cardPlay.Card is DollPart) 
            {
                cnt++;
                if (cnt>=2)
                {
                    for(int i=0;i<Amount;i++)   
                      {
                        if (base.Owner.Player.Character is AliceCharacter)
		            {
			        await CreatureCmd.TriggerAnim(base.Owner, "Summon", base.Owner.Player.Character.CastAnimDelay);
		            }
                          await ToolBox.MakeRandomDoll(Owner);
                      }
                    cnt = 0;
                }
            }
        }
    }
    

}
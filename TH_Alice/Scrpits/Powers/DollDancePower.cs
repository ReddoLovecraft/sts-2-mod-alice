
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Random;
using System.Linq;
using TH_Alice.Scrpits.Dolls;
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
            if(cardPlay.Card.Owner!=this.Owner.Player)
            return;
            Flash();
            List<Creature> dolls = cardPlay.Card.Owner.Creature.Pets
                .Where(p => p.IsAlive && p.Monster is AliceDollMonsterModel)
                .ToList();

            if (dolls.Count > 0)
            {
                CombatState? combatState = cardPlay.Card.Owner.Creature.CombatState ?? Owner.CombatState;
                if (combatState == null)
                {
                    return;
                }
                for (int i = 0; i < Amount; i++)
                {
                    Rng rng = Owner.Player.RunState.Rng.CombatCardGeneration;
                    int randomNumber = rng.NextInt(0, dolls.Count);
                    await DollTurnPhase.ExecuteSingle(combatState, dolls[randomNumber], context);
                }
            }
            else for(int i=0;i<Amount;i++)
            {
                 if (base.Owner.Player.Character is AliceCharacter)
		            {
			        await CreatureCmd.TriggerAnim(base.Owner, "Summon", base.Owner.Player.Character.CastAnimDelay);
		            }
                await ToolBox.MakeRandomDoll(Owner);
            }
        }
    }
    

}

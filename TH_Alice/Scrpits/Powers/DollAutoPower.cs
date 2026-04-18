
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Linq;
using TH_Alice.Scrpits.Dolls;
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
            CombatState? combatState = cardPlay.Card.Owner.Creature.CombatState ?? Owner.CombatState;
            if (combatState == null)
            {
                return;
            }

            var dolls = cardPlay.Card.Owner.Creature.Pets
                .Where(p => p.IsAlive && p.Monster is AliceDollMonsterModel)
                .ToList();
            if (dolls.Count == 0)
            {
                return;
            }

            for (int j = 0; j < Amount; j++)
            {
                for (int i = 0; i < dolls.Count; i++)
                {
                    await DollTurnPhase.ExecuteSingle(combatState, dolls[i], context);
                }
            }
        }
        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            await PowerCmd.Remove(this);
        }
    }
    

}

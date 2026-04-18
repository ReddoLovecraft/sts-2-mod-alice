
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Linq;
using TH_Alice.Scrpits.Dolls;
using TH_Alice.Scrpits.Main;
using TH_Alice.TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Powers
{
    public sealed class DollBowPower : AlicePowerModel
    {
        public override PowerType Type => PowerType.Debuff;
        private class Data
        {
            public readonly Dictionary<CardModel, int> amountsForPlayedCards = new Dictionary<CardModel, int>();
        }
        public override PowerStackType StackType => PowerStackType.Counter;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://ArtWorks/Powers/DBP32.png";
        public override string? CustomBigIconPath => "res://ArtWorks/Powers/DBP64.png";

        public DollBowPower() { }
        protected override object InitInternalData()
        {
            return new Data();
        }

        public override Task BeforeCardPlayed(CardPlay cardPlay)
        {
            GetInternalData<Data>().amountsForPlayedCards.Add(cardPlay.Card, base.Amount);
            return Task.CompletedTask;
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (GetInternalData<Data>().amountsForPlayedCards.Remove(cardPlay.Card, out var value))
            {
                Flash();
                var dolls = cardPlay.Card.Owner.Creature.Pets.Where(p => p.IsAlive && p.Monster is AliceDollMonsterModel).ToList();
                for (int i = 0; i < dolls.Count; i++)
                {
                    SfxCmd.Play(AliceModInit.ToModSfxPath("ArtWorks/SFX/dollbow.wav"));
                    decimal dmg = value;
                    dmg += dolls[i].GetPowerAmount<StrengthPower>();
                    if (dolls[i].HasPower<WeakPower>())
                    {
                        dmg *= 0.75m;
                    }
                    await CreatureCmd.Damage(context, base.Owner, dmg, ValueProp.Move, null, null);
                }
               
            }
        }

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            await PowerCmd.Remove(this);
        }
    }
    

}

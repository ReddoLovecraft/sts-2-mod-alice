
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Powers
{
    public sealed class RosePower : AlicePowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://ArtWorks/Powers/RP332.png";
        public override string? CustomBigIconPath => "res://ArtWorks/Powers/RP364.png";
        public RosePower() { }
        public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        {
            if (target == base.Owner && dealer != null && (props.IsPoweredAttack_() || cardSource is Omnislice))
            {
                if (Owner.HasPower<ThornsPower>()) 
                {
                    Flash();
                    await CreatureCmd.Damage(choiceContext, dealer, Owner.GetPowerAmount<ThornsPower>(), ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.SkipHurtAnim, base.Owner, null);
                }
            }
        }

    }
    

}
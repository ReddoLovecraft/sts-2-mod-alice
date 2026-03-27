
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using TH_Alice.Scrpits.Main;
using TH_Alice.TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Powers
{
    public sealed class ColorfulDollerPower : AlicePowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [(HoverTipFactory.ForEnergy(this))];
        public override string? CustomPackedIconPath => "res://ArtWorks/Powers/CDP32.png";
        public override string? CustomBigIconPath => "res://ArtWorks/Powers/CDP64.png";

        public ColorfulDollerPower() { }
        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner.Player)
            {
                return;
            }    
            await PlayerCmd.GainEnergy(ToolBox.GetDollCount(Owner), player);
            await CardPileCmd.Draw(choiceContext, ToolBox.GetDollKind(Owner), player);
        }
    }
    

}
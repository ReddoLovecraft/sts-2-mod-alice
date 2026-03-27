
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using TH_Alice.Scrpits.Cards;
using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Powers
{
    public sealed class WitchFormPower : AlicePowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override Color AmountLabelColor => PowerModel._normalAmountLabelColor;
        public override string? CustomPackedIconPath => "res://ArtWorks/Powers/WFP32.png";
        public override string? CustomBigIconPath => "res://ArtWorks/Powers/WFP64.png";

        public WitchFormPower() { }
        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner.Player)
            {
                return;
            }
            List<CardModel> list = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 0, 999), context: choiceContext, player: Owner.Player, filter: (CardModel CM) => CM.Type == CardType.Skill, source: this)).ToList();
            foreach (CardModel item in list)
            {
                CardModel cardModel = base.CombatState.CreateCard<GodReuse>(Owner.Player);
                if (item.IsUpgraded)
                {
                    CardCmd.Upgrade(cardModel);
                }
                await CardCmd.Transform(item, cardModel);
            }

            List<CardModel> list2 = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 0, 999), context: choiceContext, player: Owner.Player, filter: (CardModel CM)=>CM.Type==CardType.Attack, source: this)).ToList();
            foreach (CardModel item in list2)
            {
                CardModel cardModel = base.CombatState.CreateCard<GodGun>(Owner.Player);
                if (item.IsUpgraded)
                {
                    CardCmd.Upgrade(cardModel);
                }
                await CardCmd.Transform(item, cardModel);
            }
        }
    }
    

}
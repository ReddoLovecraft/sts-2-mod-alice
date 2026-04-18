using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Main;
using TH_Alice.Scrpits.Powers;
using TH_Alice.TH_Alice.Scrpits.Main;
using static MegaCrit.Sts2.Core.Models.Monsters.KnowledgeDemon;


namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(AliceCardPool))]
public class DollMake : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    static string text = StringHelper.Slugify("Doll");
    static LocString locString = ToolBox.L10NStatic(text + ".title");
    static LocString locString2 = ToolBox.L10NStatic(text + ".description");
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
        new HoverTip(locString,locString2)
  });
    public DollMake() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.None)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Player owner = cardPlay.Card.Owner ?? base.Owner;
        if (owner == null)
        {
            return;
        }

        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, owner);
            CardModel cardModel;
            CombatState combatState = owner.Creature.CombatState ?? base.CombatState;
            if (combatState == null)
            {
                return;
            }

        for (int i=0;i< base.DynamicVars.Cards.IntValue;i++)
        {
            CardSelectorPrefs prefs = new CardSelectorPrefs(base.SelectionScreenPrompt, 1);
            List<CardModel> cards =
            [
                combatState.CreateCard<CreatePengLai>(owner),
                combatState.CreateCard<CreateShangHai>(owner),
                combatState.CreateCard<CreateNetherland>(owner),
                combatState.CreateCard<CreateBomb>(owner),
                combatState.CreateCard<CreateCurse>(owner),
                combatState.CreateCard<CreateRussia>(owner),
                combatState.CreateCard<CreateXiZang>(owner),
                combatState.CreateCard<CreateGoliath>(owner),
                combatState.CreateCard<CreateRoundTable>(owner),
                combatState.CreateCard<CreateHina>(owner),
                combatState.CreateCard<CreateLondon>(owner),
                combatState.CreateCard<CreateOrl>(owner),
                combatState.CreateCard<CreateFrance>(owner)
            ];

            cardModel = (await CardSelectCmd.FromSimpleGrid(choiceContext, cards, owner, prefs)).FirstOrDefault();
             if(owner.Character is AliceCharacter)
            {
                await CreatureCmd.TriggerAnim(owner.Creature, "Summon", owner.Character.CastAnimDelay);
            }
            if (cardModel != null)
            {
                await ((AliceCardModel)cardModel).OnChosen();
            }
        }
    }
    protected override void OnUpgrade()
    {
       base.DynamicVars.Cards.UpgradeValueBy(1);
    }
}

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(AliceCardPool))]
public class Lazy : AliceCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
 {
         HoverTipFactory.FromPower<SlowPower>()
 });
    public Lazy() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if (!IsUpgraded) 
		{
            await PowerCmd.Apply<SlowPower>(Owner.Creature,1, base.Owner.Creature, this);
        }
	    foreach(Creature mos in Owner.Creature.CombatState.HittableEnemies) 
		{
			if (mos.IsAlive) 
			{
                await PowerCmd.Apply<SlowPower>(mos, 1, base.Owner.Creature, this);
            }
		}
        PlayerCmd.EndTurn(base.Owner, canBackOut: false);
    }
	protected override void OnUpgrade()
	{
		
	}
}

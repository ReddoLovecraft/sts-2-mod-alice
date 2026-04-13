using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Main;
using TH_Alice.Scrpits.Powers;
using TH_Alice.TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(AliceCardPool))]
public class ViolentInvultuation : AliceCardModel
{
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    static string text = StringHelper.Slugify("Doll");
    static LocString locString = ToolBox.L10NStatic(text + ".title");
    static LocString locString2 = ToolBox.L10NStatic(text + ".description");
    static string text2 = StringHelper.Slugify("Curse");
    static LocString locString3 = ToolBox.L10NStatic(text2 + ".title");
    static LocString locString4 = ToolBox.L10NStatic(text2 + ".description");
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
  {
         new HoverTip(locString3,locString4),
        new HoverTip(locString,locString2)
  });
    public ViolentInvultuation() : base(1, CardType.Skill ,CardRarity.Uncommon, TargetType.AnyEnemy)
	{
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        SfxCmd.Play(AliceModInit.ToModSfxPath("ArtWorks/SFX/cast.wav"));
        List<PowerModel> to_remove = new List<PowerModel>();
        foreach(PowerModel buff in cardPlay.Target.Powers)
        {
            if(buff.Type==PowerType.Buff)
            {
                to_remove.Add(buff);
            }
        }
        for(int i=0;i<to_remove.Count;i++)
        {
        if(Owner.Character is AliceCharacter)
        {
                await CreatureCmd.TriggerAnim(base.Owner.Creature, "Summon", base.Owner.Character.CastAnimDelay);
        }
          await ToolBox.MakeDoll<CursePower>(Owner.Creature);
        }
        for(int i=to_remove.Count-1;i>=0;i--)
        {
            await PowerCmd.Remove(to_remove[i]);
            to_remove.RemoveAt(i);
        }
    }
	protected override void OnUpgrade()
	{
        base.EnergyCost.UpgradeBy(-1);
    }
}

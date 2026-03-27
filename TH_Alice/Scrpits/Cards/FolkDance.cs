using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
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
public class FolkDance : AliceCardModel
{
    
    protected override bool HasEnergyCostX => true;
    static string text = StringHelper.Slugify("Doll");
    static LocString locString = ToolBox.L10NStatic(text + ".title");
    static LocString locString2 = ToolBox.L10NStatic(text + ".description");
   
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
        new HoverTip(locString,locString2)
  });
    public FolkDance() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.None)
	{
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        int num = 0 + ResolveEnergyXValue();
        if (IsUpgraded) num +=1;
        for (int i = 0; i < num; i++)
        {
            List<AlicePowerModel> apms=new List<AlicePowerModel>();
            foreach (PowerModel pm in Owner.Creature.Powers) 
            {
                if (pm is AlicePowerModel apm&& apm.IsDollPower)
                {
                  apms.Add(apm);
                }
            }
            //防止自爆人偶执行逻辑后自己删除导致循环越界
            for(int j=apms.Count-1;j>=0;j--)
            {
                await apms[j].DollAction(choiceContext);
                apms.RemoveAt(j);
            }
        }
    }
	protected override void OnUpgrade()
	{
        base.EnergyCost.UpgradeBy(-1);
    }
}

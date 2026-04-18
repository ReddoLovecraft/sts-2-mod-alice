using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Alice.Scrpits.Main;
using TH_Alice.Scrpits.Character;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using TH_Alice.TH_Alice.Scrpits.Main;
using MegaCrit.Sts2.Core.HoverTips;
using TH_Alice.Scrpits.Powers;
using TH_Alice.Scrpits.Dolls;

namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(AliceCardPool))]
public class ShangHaiAttack : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
  static string text = StringHelper.Slugify("Doll");
  static LocString locString = ToolBox.L10NStatic(text + ".title");
  static LocString locString2 = ToolBox.L10NStatic(text + ".description");
  static string text2 = StringHelper.Slugify("Shanghai");
  static LocString locString3 = ToolBox.L10NStatic(text2 + ".title");
  static LocString locString4 = ToolBox.L10NStatic(text2 + ".description");
  protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[2]
{
     new HoverTip(locString3,locString4),
      new HoverTip(locString,locString2)
});
	public ShangHaiAttack() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.Self)
	{
	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if(Owner.Character is AliceCharacter)
        {
                await CreatureCmd.TriggerAnim(base.Owner.Creature, "Summon", base.Owner.Character.CastAnimDelay);
        }
		await ToolBox.MakeDoll<ShangHaiPower>(Owner.Creature);
		var dolls = Owner.Creature.Pets.Where(p => p.IsAlive && p.Monster is SHANGHAI).ToList();
        for (int j = 0; j < dolls.Count; j++)
        {
                await DollTurnPhase.ExecuteSingle(CombatState!, dolls[j], choiceContext);
        }
	}
	protected override void OnUpgrade()
	{
		this.EnergyCost.UpgradeBy(-1);
	}
}

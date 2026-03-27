using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(AliceCardPool))]
public class AliceKiller : AliceCardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8, ValueProp.Move)];
	public AliceKiller() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
	{
	}
	    protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
  {
        HoverTipFactory.FromPower<StrengthPower>()
  });
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
	
			Color color = new Color(1, 0.7137255f, 0.75686276f, 1);
			double num2 = ((SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast) ? 0.2 : 0.3);
			NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NHorizontalLinesVfx.Create(color, 0.8 + 8 * num2));
			SfxCmd.Play("event:/sfx/characters/ironclad/ironclad_whirlwind");
			NRun.Instance?.GlobalUi.AddChildSafely(NSmokyVignetteVfx.Create(color, color));
		    foreach(Creature mos in Owner.Creature.CombatState.HittableEnemies) 
		{
			if (mos.IsAlive) 
			{
                AttackCommand attackCommand = await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(mos)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
				if (mos.IsAlive&&mos.IsHittable) 
				{
                    int dmg = attackCommand.Results.Sum((DamageResult r) => r.TotalDamage + r.OverkillDamage);
					await PowerCmd.Apply<PiercingWailPower>(mos,dmg,Owner.Creature,this);
                }
            }
		}
	}
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(3); 
	}
}

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Cards;
[Pool(typeof(AliceCardPool))]
public class ReturnInanimateness : AliceCardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [(CardKeyword.Ethereal)];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(18, ValueProp.Move)];
	public ReturnInanimateness() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
	{

	}
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(base.CombatState)
            .WithAttackerAnim("Cast", 0.5f)
            .BeforeDamage(async delegate
            {
                List<Creature> enemies = base.CombatState.Enemies.Where((Creature e) => e.IsAlive).ToList();
                NHyperbeamVfx nHyperbeamVfx = NHyperbeamVfx.Create(base.Owner.Creature, enemies.Last());
                if (nHyperbeamVfx != null)
                {
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamVfx);
                    await Cmd.Wait(0.5f);
                }
                foreach (Creature item in enemies)
                {
                    NHyperbeamImpactVfx nHyperbeamImpactVfx = NHyperbeamImpactVfx.Create(base.Owner.Creature, item);
                    if (nHyperbeamImpactVfx != null)
                    {
                        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(nHyperbeamImpactVfx);
                    }
                }
            })
            .Execute(choiceContext);
        SfxCmd.Play(AliceModInit.ToModSfxPath("ArtWorks/SFX/bomb.wav"));
    }
	protected override void OnUpgrade()
	{
		DynamicVars.Damage.UpgradeValueBy(6); 
	}
}

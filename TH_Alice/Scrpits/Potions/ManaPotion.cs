using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;
using TH_Alice.Scrpits.Character;
using TH_Alice.Scrpits.Powers;

namespace TH_Alice.Scrpits.Potions;
[Pool(typeof(AlicePotionPool))]
public sealed class ManaPotion : CustomPotionModel
{
    public override PotionRarity Rarity => PotionRarity.Common;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    public override bool CanBeGeneratedInCombat => true;

    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];
    protected override IEnumerable<DynamicVar> CanonicalVars => (new DynamicVar[2]
    {
        new EnergyVar(1),
        new PowerVar<ManaLosePower>(2m)
    });
    public override string? CustomPackedImagePath => "res://ArtWorks/Potion/MANA_POTION.png";
    public override string? CustomPackedOutlinePath => "res://ArtWorks/Potion/Outlines/MANA_POTION.png";
    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        await PlayerCmd.SetEnergy(Owner.Creature.Player.MaxEnergy, Owner.Creature.Player);
        await PowerCmd.Apply<ManaLosePower>(Owner.Creature, base.DynamicVars["ManaLosePower"].BaseValue, base.Owner.Creature, null);
    }
}

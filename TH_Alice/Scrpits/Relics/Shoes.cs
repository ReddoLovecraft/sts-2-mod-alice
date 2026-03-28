using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using TH_Alice.Scrpits.Character;
    [Pool(typeof(AliceRelicPool))]
    public class Shoes : CustomRelicModel
    {
        public override RelicRarity Rarity => RelicRarity.Rare;
        public override string PackedIconPath => $"res://ArtWorks/Relics/{Id.Entry}.png";
        protected override string PackedIconOutlinePath => $"res://ArtWorks/Relics/Outlines/{Id.Entry}.png";
        protected override string BigIconPath => $"res://ArtWorks/Relics/{Id.Entry}.png";
        protected override IEnumerable<IHoverTip> ExtraHoverTips => (new IHoverTip[1]
        {
          StunIntent.GetStaticHoverTip()
        });
        public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
        {
           if (side == base.Owner.Creature.Side && combatState.RoundNumber <= 1)
		{
			Flash();
			foreach(Creature mos in combatState.HittableEnemies)
            {
                if(mos.IsAlive)
                {
                    await CreatureCmd.Stun(mos);
                }
            }
		}
        }
       

    }



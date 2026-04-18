using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using System.Linq;
using TH_Alice.Scrpits.Dolls;


namespace TH_Alice.Scrpits.Main
{
    public abstract class AliceCardModel : CustomCardModel
    {
        public override string PortraitPath => $"res://ArtWorks/Cards/{Id.Entry}.png";
        public virtual bool IsTargetDoll => false;
        public override TargetType TargetType => IsTargetDoll ? MegaCrit.Sts2.Core.Entities.Cards.TargetType.AnyAlly : base.TargetType;
        public Creature? LastDollTarget { get; internal set; }
        public AliceCardModel(int baseCost, CardType type, CardRarity rarity, TargetType target, bool showInCardLibrary = true, bool autoAdd = true)
     : base(baseCost, type, rarity, target, showInCardLibrary)
        {
            if (autoAdd)
            {
                CustomContentDictionary.AddModel(GetType());
            }
        }

        protected override bool IsPlayable
        {
            get
            {
                if (IsTargetDoll)
                {
                    return Owner?.Creature?.Pets.Any(p => p.IsAlive && p.Monster is AliceDollMonsterModel) == true;
                }
                return base.IsPlayable;
            }
        }

        public virtual async Task OnChosen() { }
    }
  
}

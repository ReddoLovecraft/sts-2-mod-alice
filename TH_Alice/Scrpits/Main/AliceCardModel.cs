using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;


namespace TH_Alice.Scrpits.Main
{
    public abstract class AliceCardModel : CustomCardModel
    {
        public override string PortraitPath => $"res://ArtWorks/Cards/{Id.Entry}.png";
        public AliceCardModel(int baseCost, CardType type, CardRarity rarity, TargetType target, bool showInCardLibrary = true, bool autoAdd = true)
     : base(baseCost, type, rarity, target, showInCardLibrary)
        {
            if (autoAdd)
            {
                CustomContentDictionary.AddModel(GetType());
            }
        }

        public virtual async Task OnChosen() { }
    }
  
}

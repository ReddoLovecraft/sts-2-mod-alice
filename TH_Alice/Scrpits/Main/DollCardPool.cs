using BaseLib.Abstracts;
using Godot;
using Patchouib.Scrpits.Main;

namespace TH_Alice.Scrpits.Character
{
	public class DollCardPool : CustomCardPoolModel,IVisibleCardPool
{
	public override string Title => "Doll";

    public override Color ShaderColor => new Color(1, 1, 0, 1);
	public override Color DeckEntryCardColor => new Color(1, 0.84313726f, 0, 1);
  	public override string? BigEnergyIconPath => "res://ArtWorks/Character/card_orb.png";
    public override string? TextEnergyIconPath => "res://ArtWorks/Character/cost_orb.png";
    public override bool IsColorless => false;
	public string GetCardLibraryIconPath()
    {
            return "res://ArtWorks/Character/pool_icon.png";
    }
	public string? GetCardLibraryHoverTipKey() => "Doll";
}
}

using BaseLib.Abstracts;
using Godot;

namespace TH_Alice.Scrpits.Character
{
	public class AliceCardPool : CustomCardPoolModel
{
	public override string Title => "TH_Alice";

    public override Color ShaderColor => new Color(1, 1, 0, 1);
	public override Color DeckEntryCardColor => new Color(1, 0.84313726f, 0, 1);
  	public override string? BigEnergyIconPath => "res://ArtWorks/Character/card_orb.png";
    public override string? TextEnergyIconPath => "res://ArtWorks/Character/cost_orb.png";
        public override bool IsColorless => false;
}
}

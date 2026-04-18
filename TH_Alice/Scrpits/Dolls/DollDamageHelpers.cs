using MegaCrit.Sts2.Core.Entities.Creatures;

namespace TH_Alice.Scrpits.Dolls;

public static class DollDamageHelpers
{
	[System.ThreadStatic]
	public static bool SuppressDollPetOwnerDuringDamage;
}

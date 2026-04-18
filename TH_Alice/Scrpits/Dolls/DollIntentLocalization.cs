using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using System.Collections.Generic;

namespace TH_Alice.Scrpits.Dolls;

public static class DollIntentLocalization
{
	private static bool _installRequested;
	private static bool _subscribed;

	private static readonly string[] _dollIds =
	[
		"SHANGHAI",
		"PENGLAI",
		"XIZANG",
		"HINA",
		"GOLIATH",
		"ROUNDTABLE",
		"FRANCE",
		"ORL",
		"NETHERLAND",
		"RUSSIA",
		"BOMB",
		"LONDON",
		"CURSE"
	];

	public static void Install()
	{
		_installRequested = true;
		TryInstallNow();
	}

	private static void TryInstallNow()
	{
		if (!_installRequested)
		{
			return;
		}
		if (LocManager.Instance == null)
		{
			return;
		}
		Apply();
		if (!_subscribed)
		{
			LocManager.Instance.SubscribeToLocaleChange(Apply);
			_subscribed = true;
		}
	}

	private static void Apply()
	{
		if (LocManager.Instance == null)
		{
			return;
		}
		LocTable monsters = LocManager.Instance.GetTable("monsters");
		LocTable intents = LocManager.Instance.GetTable("intents");

		Dictionary<string, string> patch = new Dictionary<string, string>();
		foreach (string id in _dollIds)
		{
			CopyKey(monsters, patch, id + ".moves.INTENT.title");
			CopyKey(monsters, patch, id + ".moves.INTENT.description");
			CopyKey(monsters, patch, id + ".moves.MAXINTENT.title");
			CopyKey(monsters, patch, id + ".moves.MAXINTENT.description");
		}
		intents.MergeWith(patch);
	}

	private static void CopyKey(LocTable from, Dictionary<string, string> to, string key)
	{
		if (from.HasEntry(key))
		{
			to[key] = from.GetRawText(key);
		}
	}
}

[HarmonyPatch(typeof(LocManager), "Initialize")]
public static class DollIntentLocalization_LocManagerInitializePatch
{
	public static void Postfix()
	{
		DollIntentLocalization.Install();
	}
}

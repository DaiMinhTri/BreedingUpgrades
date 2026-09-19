using BreedingUpgrades.Utils;
using HarmonyLib;
using UnityEngine;

namespace BreedingUpgrades.Patches;

[HarmonyPatch(typeof(ItemDrop), "SetQuality")]
public static class ItemDropSetQualityPatch
{
	[HarmonyPrefix]
	public static void Prefix(ItemDrop __instance, ref int quality)
	{
		if (ProcreationContext.IsInProcreation && BreedingUpgradesPlugin.EnableMod.Value && IsEggItem(__instance))
		{
			BreedingUpgradesPlugin.LogDebug(string.Format("Egg SetQuality intercepted: {0}, quality={1}", __instance.m_itemData?.m_shared?.m_name ?? "unknown", quality));
			int num = LevelCalculator.ApplyEggUpgrade(quality);
			if (num != quality)
			{
				quality = num;
			}
		}
	}

	private static bool IsEggItem(ItemDrop item)
	{
		if (item == null || item.m_itemData == null || item.m_itemData.m_shared == null)
		{
			return false;
		}
		string name = item.m_itemData.m_shared.m_name;
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}
		return name.ToLowerInvariant().Contains("egg");
	}
}

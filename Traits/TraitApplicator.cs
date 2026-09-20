using HarmonyLib;
using UnityEngine;

namespace BreedingUpgrades.Traits;

[HarmonyPatch(typeof(Character), "Awake")]
public static class TraitApplicator_Awake
{
	[HarmonyPostfix]
	private static void Postfix(Character __instance)
	{
		if (__instance.IsPlayer())
		{
			return;
		}

		if (__instance.m_nview == null || !__instance.m_nview.IsValid())
		{
			return;
		}

		TraitInitializer.InitializeTraitsIfMissing(__instance);
		ApplySizeTrait(__instance);
	}

	public static void ApplySizeTrait(Character character)
	{
		if (!BreedingUpgradesPlugin.EnableSizeTrait.Value)
		{
			return;
		}

		if (character.m_nview == null || !character.m_nview.IsValid())
		{
			return;
		}

		ZDO zdo = character.m_nview.GetZDO();
		if (zdo == null || zdo.GetFloat(TraitKeys.SIZE, -1f) < 0f)
		{
			return;
		}

		float multiplier = TraitInitializer.GetSizeMultiplier(zdo);
		character.transform.localScale = Vector3.one * multiplier;
		BreedingUpgradesPlugin.LogDebug($"Applied size trait to {character.gameObject.name}: {multiplier:F2}x");
	}
}

[HarmonyPatch(typeof(Character), "SetLevel")]
public static class TraitApplicator_SetLevel
{
	[HarmonyPostfix]
	private static void Postfix(Character __instance)
	{
		if (__instance.IsPlayer())
		{
			return;
		}

		TraitApplicator_Awake.ApplySizeTrait(__instance);
	}
}

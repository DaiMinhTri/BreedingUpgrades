using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BreedingUpgrades.Utils;
using HarmonyLib;
using UnityEngine;

namespace BreedingUpgrades.Patches;

[HarmonyPatch(typeof(Procreation), "Procreate")]
public static class ProcreationPatch
{
	private static readonly Dictionary<string, string> SpeciesMap = new Dictionary<string, string>
	{
		{ "Boar", "Boar" },
		{ "Wolf", "Wolf" },
		{ "Lox", "Lox" },
		{ "Chicken", "Chicken" },
		{ "Moose", "Moose" },
		{ "Asksvin", "Asksvin" }
	};

	[HarmonyPrefix]
	public static void Prefix(Procreation __instance)
	{
		ProcreationContext.IsInProcreation = true;
		Character component = __instance.GetComponent<Character>();
		ProcreationContext.ParentLevel = ((component == null) ? 1 : component.GetLevel());
		BreedingUpgradesPlugin.LogDebug($"Procreation started - Parent level: {ProcreationContext.ParentLevel}");

		if (BreedingUpgradesPlugin.EnableBreedingLimit.Value && component != null)
		{
			string species = DetectSpecies(component);
			if (!string.IsNullOrEmpty(species))
			{
				int maxCreatures = BreedingUpgradesPlugin.GetMaxCreatures(species);
				__instance.m_maxCreatures = maxCreatures;
				BreedingUpgradesPlugin.LogDebug($"Breeding limit applied: {species} max {maxCreatures}");
			}
		}
	}

	private static string DetectSpecies(Character character)
	{
		string characterName = character.gameObject.name;
		foreach (var kvp in SpeciesMap)
		{
			if (characterName.IndexOf(kvp.Key, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return kvp.Value;
			}
		}
		return null;
	}

	[HarmonyPostfix]
	public static void Postfix()
	{
		ProcreationContext.IsInProcreation = false;
		BreedingUpgradesPlugin.LogDebug("Procreation ended");
	}

	[HarmonyTranspiler]
	public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		MethodInfo setLevelMethod = AccessTools.Method(typeof(Character), "SetLevel", null, null);
		MethodInfo mutationMethod = AccessTools.Method(typeof(LevelCalculator), "ApplyMutation", null, null);
		List<CodeInstruction> codes = instructions.ToList();
		int patchCount = 0;
		for (int i = 0; i < codes.Count; i++)
		{
			if (CodeInstructionExtensions.Calls(codes[i], setLevelMethod))
			{
				yield return new CodeInstruction(OpCodes.Call, mutationMethod);
				patchCount++;
			}
			yield return codes[i];
		}
		if (patchCount != 1)
		{
			BreedingUpgradesPlugin.LogError($"Expected 1 SetLevel call in Procreation.Procreate, found {patchCount}. Valheim may have updated!");
		}
		else
		{
			BreedingUpgradesPlugin.LogDebug("Procreation transpiler applied successfully");
		}
	}
}

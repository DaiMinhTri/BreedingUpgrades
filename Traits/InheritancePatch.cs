using BreedingUpgrades.Patches;
using HarmonyLib;
using UnityEngine;

namespace BreedingUpgrades.Traits;

[HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Instantiate), typeof(GameObject), typeof(Vector3), typeof(Quaternion))]
public static class InheritancePatch
{
	[HarmonyPostfix]
	private static void Postfix(ref GameObject __result)
	{
		if (!BreedingUpgradesPlugin.EnableSizeTrait.Value)
		{
			return;
		}

		if (!ProcreationContext.IsInProcreation)
		{
			return;
		}

		if (__result == null)
		{
			return;
		}

		Character childChar = __result.GetComponent<Character>();
		if (childChar == null)
		{
			return;
		}

		if (childChar.m_nview == null || !childChar.m_nview.IsValid())
		{
			return;
		}

		ZDO childZDO = childChar.m_nview.GetZDO();
		if (childZDO == null)
		{
			return;
		}

		float sizeA = GetSize(ProcreationContext.ParentA);
		float sizeB = GetSize(ProcreationContext.ParentB);

		float blended = (sizeA + sizeB) / 2f;

		float variance = BreedingUpgradesPlugin.SizeTraitVariance.Value / 100f;
		float randomVariance = Random.Range(-variance, variance);
		blended *= (1f + randomVariance);

		float mutationChance = BreedingUpgradesPlugin.SizeTraitMutationChance.Value / 100f;
		if (Random.Range(0f, 1f) < mutationChance)
		{
			int minSize = BreedingUpgradesPlugin.SizeTraitMin.Value;
			int maxSize = BreedingUpgradesPlugin.SizeTraitMax.Value;
			float minF = minSize / 128f;
			float maxF = maxSize / 128f;
			float mutationRange = (maxF - minF) * 0.2f;
			blended += Random.Range(-mutationRange, mutationRange);
		}

		int min = BreedingUpgradesPlugin.SizeTraitMin.Value;
		int max = BreedingUpgradesPlugin.SizeTraitMax.Value;
		blended = Mathf.Clamp(blended, min / 128f, max / 128f);

		childZDO.Set(TraitKeys.SIZE, blended * 128f);

		int genA = GetGeneration(ProcreationContext.ParentA);
		int genB = GetGeneration(ProcreationContext.ParentB);
		childZDO.Set(TraitKeys.GENERATION, Mathf.Max(genA, genB) + 1);

		childChar.transform.localScale = Vector3.one * blended;

		string parentAName = ProcreationContext.ParentA != null ? ProcreationContext.ParentA.name : "?";
		string parentBName = ProcreationContext.ParentB != null && ProcreationContext.ParentB != ProcreationContext.ParentA
			? ProcreationContext.ParentB.name : parentAName;
		BreedingUpgradesPlugin.LogDebug($"Inherited size: {parentAName}({sizeA / 128f:F2}x) + {parentBName}({sizeB / 128f:F2}x) -> {childChar.gameObject.name}({blended:F2}x)");
	}

	private static float GetSize(GameObject go)
	{
		if (go == null)
		{
			return 128f;
		}

		Character character = go.GetComponent<Character>();
		if (character?.m_nview == null || !character.m_nview.IsValid())
		{
			return 128f;
		}

		ZDO zdo = character.m_nview.GetZDO();
		return zdo?.GetFloat(TraitKeys.SIZE, 128f) ?? 128f;
	}

	private static int GetGeneration(GameObject go)
	{
		if (go == null)
		{
			return 1;
		}

		Character character = go.GetComponent<Character>();
		if (character?.m_nview == null || !character.m_nview.IsValid())
		{
			return 1;
		}

		ZDO zdo = character.m_nview.GetZDO();
		return zdo?.GetInt(TraitKeys.GENERATION, 1) ?? 1;
	}
}

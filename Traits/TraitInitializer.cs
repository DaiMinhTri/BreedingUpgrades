using UnityEngine;

namespace BreedingUpgrades.Traits;

public static class TraitInitializer
{
	public static void InitializeTraitsIfMissing(Character character)
	{
		if (!BreedingUpgradesPlugin.EnableSizeTrait.Value)
		{
			return;
		}

		if (character.IsPlayer())
		{
			return;
		}

		if (character.m_nview == null || !character.m_nview.IsValid())
		{
			return;
		}

		if (!character.m_nview.IsOwner())
		{
			return;
		}

		ZDO zdo = character.m_nview.GetZDO();
		if (zdo == null)
		{
			return;
		}

		if (zdo.GetFloat(TraitKeys.SIZE, -1f) >= 0f)
		{
			return;
		}

		int minSize = BreedingUpgradesPlugin.SizeTraitMin.Value;
		int maxSize = BreedingUpgradesPlugin.SizeTraitMax.Value;
		float sizeValue = Random.Range(minSize, maxSize + 1);
		zdo.Set(TraitKeys.SIZE, sizeValue);
		zdo.Set(TraitKeys.GENERATION, 1);
		BreedingUpgradesPlugin.LogDebug($"Initialized size trait for {character.gameObject.name}: {sizeValue} ({sizeValue / 128f:F2}x)");
	}

	public static float GetSizeMultiplier(ZDO zdo)
	{
		if (zdo == null)
		{
			return 1f;
		}

		float sizeValue = zdo.GetFloat(TraitKeys.SIZE, 128f);
		return sizeValue / 128f;
	}
}

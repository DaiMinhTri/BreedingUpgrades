using System;
using UnityEngine;

namespace BreedingUpgrades.Utils;

public static class LevelCalculator
{
	public const int VANILLA_MAX_LEVEL = 3;

	public static int GetGameMaxLevel()
	{
		if (ModDetection.IsAnyLevelModInstalled())
		{
			int num = BreedingUpgradesPlugin.MaxStarLevel.Value + 1;
			BreedingUpgradesPlugin.LogDebug($"{ModDetection.GetDetectedLevelMod()} detected - using config max level {num}");
			return num;
		}
		return 3;
	}

	public static int GetEffectiveMaxLevel()
	{
		int value = BreedingUpgradesPlugin.MaxStarLevel.Value;
		int gameMaxLevel = GetGameMaxLevel();
		return Math.Min(value + 1, gameMaxLevel);
	}

	public static int ApplyMutation(int originalLevel)
	{
		if (!BreedingUpgradesPlugin.EnableMod.Value)
		{
			return originalLevel;
		}
		int effectiveMaxLevel = GetEffectiveMaxLevel();
		if (originalLevel >= effectiveMaxLevel)
		{
			BreedingUpgradesPlugin.LogDebug($"Creature already at max level ({effectiveMaxLevel}), no upgrade possible");
			return originalLevel;
		}
		float num = UnityEngine.Random.Range(0f, 100f);
		float value = BreedingUpgradesPlugin.UpgradeChance.Value;
		if (num <= value)
		{
			int num2 = originalLevel + 1;
			int gameMaxLevel = GetGameMaxLevel();
			if (num2 > gameMaxLevel)
			{
				BreedingUpgradesPlugin.LogDebug($"Mutation would exceed game max ({gameMaxLevel}). Capping. Install CLLC for higher stars!");
				num2 = gameMaxLevel;
			}
			if (num2 > originalLevel)
			{
				int num3 = originalLevel - 1;
				int num4 = num2 - 1;
				BreedingUpgradesPlugin.LogInfo($"\ud83e\uddec Offspring mutated! {num3}★ → {num4}★");
			}
			return num2;
		}
		BreedingUpgradesPlugin.LogDebug($"No mutation (roll {num:F1} > {value}%)");
		return originalLevel;
	}

	public static int ApplyEggUpgrade(int originalQuality)
	{
		if (!BreedingUpgradesPlugin.EnableMod.Value)
		{
			return originalQuality;
		}
		int value = BreedingUpgradesPlugin.MaxStarLevel.Value;
		if (originalQuality - 1 >= value)
		{
			BreedingUpgradesPlugin.LogDebug($"Egg already at max quality ({value} stars), no upgrade possible");
			return originalQuality;
		}
		float num = UnityEngine.Random.Range(0f, 100f);
		float value2 = BreedingUpgradesPlugin.UpgradeChance.Value;
		if (num <= value2)
		{
			int num2 = originalQuality + 1;
			int num3 = originalQuality - 1;
			int num4 = num2 - 1;
			BreedingUpgradesPlugin.LogInfo($"\ud83e\udd5a Egg upgraded! {num3}★ → {num4}★");
			return num2;
		}
		BreedingUpgradesPlugin.LogDebug($"No egg upgrade (roll {num:F1} > {value2}%)");
		return originalQuality;
	}
}

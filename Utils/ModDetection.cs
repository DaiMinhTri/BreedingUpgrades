using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Bootstrap;

namespace BreedingUpgrades.Utils;

public static class ModDetection
{
	private static bool? cllcInstalled;

	private static bool? anyLevelModInstalled;

	private static string detectedLevelMod;

	private const string CLLC_GUID = "org.bepinex.plugins.creaturelevelcontrol";

	private const string STAR_LEVEL_SYSTEM_GUID = "blacks7ar.StarLevelSystem";

	public static bool IsCLLCInstalled()
	{
		if (cllcInstalled.HasValue)
		{
			return cllcInstalled.Value;
		}
		cllcInstalled = Chainloader.PluginInfos.ContainsKey("org.bepinex.plugins.creaturelevelcontrol");
		if (cllcInstalled.Value)
		{
			detectedLevelMod = "CLLC";
			BreedingUpgradesPlugin.LogDebug("CLLC detected via BepInEx plugin system");
		}
		return cllcInstalled.Value;
	}

	public static bool IsAnyLevelModInstalled()
	{
		if (anyLevelModInstalled.HasValue)
		{
			return anyLevelModInstalled.Value;
		}
		try
		{
			Dictionary<string, PluginInfo> pluginInfos = Chainloader.PluginInfos;
			if (pluginInfos.ContainsKey("org.bepinex.plugins.creaturelevelcontrol"))
			{
				anyLevelModInstalled = true;
				detectedLevelMod = "CLLC";
				BreedingUpgradesPlugin.LogInfo("✓ CLLC detected - extended star levels available");
				return true;
			}
			if (pluginInfos.ContainsKey("blacks7ar.StarLevelSystem"))
			{
				anyLevelModInstalled = true;
				detectedLevelMod = "StarLevelSystem";
				BreedingUpgradesPlugin.LogInfo("✓ StarLevelSystem detected - extended star levels available");
				return true;
			}
			BreedingUpgradesPlugin.LogDebug("No creature level mod detected - using vanilla level cap (2 stars max)");
			anyLevelModInstalled = false;
			return false;
		}
		catch (Exception ex)
		{
			BreedingUpgradesPlugin.LogDebug("Error checking for level mods: " + ex.Message);
			anyLevelModInstalled = false;
			return false;
		}
	}

	public static string GetDetectedLevelMod()
	{
		IsAnyLevelModInstalled();
		return detectedLevelMod ?? "None";
	}

	public static void ResetCache()
	{
		cllcInstalled = null;
		anyLevelModInstalled = null;
		detectedLevelMod = null;
	}
}

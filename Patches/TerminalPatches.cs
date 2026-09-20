using System;
using BreedingUpgrades.Utils;
using HarmonyLib;
using UnityEngine;
using static Terminal;

namespace BreedingUpgrades.Patches;

[HarmonyPatch(typeof(Terminal), "Awake")]
public static class TerminalPatches
{
	private static bool commandsRegistered;

	[HarmonyPostfix]
	public static void Postfix()
	{
		if (commandsRegistered)
		{
			return;
		}
		commandsRegistered = true;
		new ConsoleCommand("breeding_info", "Display BreedingUpgrades mod status and configuration",
			(ConsoleEvent)delegate { PrintInfo(); },
			isCheat: false, isNetwork: false, onlyServer: false, isSecret: false, allowInDevBuild: false,
			hideBehindDevCommands: false, optionsFetcher: null, alwaysRefreshTabOptions: false,
			remoteCommand: false, onlyAdmin: false);
		new ConsoleCommand("breeding_simulate", "Simulate breeding rolls. Usage: breeding_simulate [count]",
			(ConsoleEvent)delegate(ConsoleEventArgs args)
			{
				int count = 100;
				if (args.Length > 1 && int.TryParse(args[1], out var result))
				{
					count = result;
				}
				SimulateRolls(count);
			},
			isCheat: false, isNetwork: false, onlyServer: false, isSecret: false, allowInDevBuild: false,
			hideBehindDevCommands: false, optionsFetcher: null, alwaysRefreshTabOptions: false,
			remoteCommand: false, onlyAdmin: false);
		new ConsoleCommand("breeding_debug", "Toggle debug logging for BreedingUpgrades",
			(ConsoleEvent)delegate
			{
				BreedingUpgradesPlugin.DebugLogging.Value = !BreedingUpgradesPlugin.DebugLogging.Value;
				Console.instance.Print("BreedingUpgrades debug logging: " + (BreedingUpgradesPlugin.DebugLogging.Value ? "ON" : "OFF"));
			},
			isCheat: false, isNetwork: false, onlyServer: false, isSecret: false, allowInDevBuild: false,
			hideBehindDevCommands: false, optionsFetcher: null, alwaysRefreshTabOptions: false,
			remoteCommand: false, onlyAdmin: false);
		BreedingUpgradesPlugin.LogDebug("Console commands registered");
	}

	private static void PrintInfo()
	{
		Console.instance.Print("=== BreedingUpgrades Info ===");
		Console.instance.Print("Version: " + BreedingUpgradesPlugin.PLUGIN_VERSION);
		Console.instance.Print($"Mod Enabled: {BreedingUpgradesPlugin.EnableMod.Value}");
		Console.instance.Print($"Upgrade Chance: {BreedingUpgradesPlugin.UpgradeChance.Value}%");
		Console.instance.Print($"Max Star Level (Config): {BreedingUpgradesPlugin.MaxStarLevel.Value}");
		Console.instance.Print($"Game Max Level: {LevelCalculator.GetGameMaxLevel()} (Level 3 = 2 stars vanilla)");
		Console.instance.Print($"Effective Max Level: {LevelCalculator.GetEffectiveMaxLevel()}");
		Console.instance.Print("Level Mod Detected: " + ModDetection.GetDetectedLevelMod());
		Console.instance.Print("Debug Logging: " + (BreedingUpgradesPlugin.DebugLogging.Value ? "ON" : "OFF"));
		Console.instance.Print("--- Breeding Limits ---");
		Console.instance.Print($"Breeding Limit: {(BreedingUpgradesPlugin.EnableBreedingLimit.Value ? "Enabled" : "Disabled")}");
		Console.instance.Print($"  Boar: {BreedingUpgradesPlugin.GetMaxCreatures("Boar")} | Wolf: {BreedingUpgradesPlugin.GetMaxCreatures("Wolf")} | Lox: {BreedingUpgradesPlugin.GetMaxCreatures("Lox")}");
		Console.instance.Print($"  Chicken: {BreedingUpgradesPlugin.GetMaxCreatures("Chicken")} | Moose: {BreedingUpgradesPlugin.GetMaxCreatures("Moose")} | Asksvin: {BreedingUpgradesPlugin.GetMaxCreatures("Asksvin")}");
		Console.instance.Print("--- Search Radius ---");
		Console.instance.Print("  Population Check Range:");
		Console.instance.Print($"    Boar: {BreedingUpgradesPlugin.GetPopulationCheckRange("Boar")}m | Wolf: {BreedingUpgradesPlugin.GetPopulationCheckRange("Wolf")}m | Lox: {BreedingUpgradesPlugin.GetPopulationCheckRange("Lox")}m");
		Console.instance.Print($"    Chicken: {BreedingUpgradesPlugin.GetPopulationCheckRange("Chicken")}m | Moose: {BreedingUpgradesPlugin.GetPopulationCheckRange("Moose")}m | Asksvin: {BreedingUpgradesPlugin.GetPopulationCheckRange("Asksvin")}m");
		Console.instance.Print("  Partner Check Range:");
		Console.instance.Print($"    Boar: {BreedingUpgradesPlugin.GetPartnerCheckRange("Boar")}m | Wolf: {BreedingUpgradesPlugin.GetPartnerCheckRange("Wolf")}m | Lox: {BreedingUpgradesPlugin.GetPartnerCheckRange("Lox")}m");
		Console.instance.Print($"    Chicken: {BreedingUpgradesPlugin.GetPartnerCheckRange("Chicken")}m | Moose: {BreedingUpgradesPlugin.GetPartnerCheckRange("Moose")}m | Asksvin: {BreedingUpgradesPlugin.GetPartnerCheckRange("Asksvin")}m");
		Console.instance.Print("--- Traits ---");
		Console.instance.Print($"Size Trait: {(BreedingUpgradesPlugin.EnableSizeTrait.Value ? "Enabled" : "Disabled")}");
		if (BreedingUpgradesPlugin.EnableSizeTrait.Value)
		{
			Console.instance.Print($"  Range: {BreedingUpgradesPlugin.SizeTraitMin.Value}%-{BreedingUpgradesPlugin.SizeTraitMax.Value}%");
			Console.instance.Print($"  Inheritance Variance: {BreedingUpgradesPlugin.SizeTraitVariance.Value}%");
			Console.instance.Print($"  Mutation Chance: {BreedingUpgradesPlugin.SizeTraitMutationChance.Value}%");
		}
		Console.instance.Print("=============================");
	}

	private static void SimulateRolls(int count)
	{
		Console.instance.Print($"=== Simulating {count} Breeding Rolls ===");
		Console.instance.Print($"Upgrade Chance: {BreedingUpgradesPlugin.UpgradeChance.Value}%");
		int num = 0;
		float value = BreedingUpgradesPlugin.UpgradeChance.Value;
		for (int i = 0; i < count; i++)
		{
			if (UnityEngine.Random.Range(0f, 100f) <= value)
			{
				num++;
			}
		}
		float num2 = (float)num / (float)count * 100f;
		Console.instance.Print($"Results: {num}/{count} upgrades ({num2:F1}%)");
		Console.instance.Print($"Expected: ~{value}%");
		float num3 = (float)count * (value / 100f);
		float num4 = (float)num - num3;
		Console.instance.Print(string.Format("Deviation from expected: {0}{1:F1}", (num4 >= 0f) ? "+" : "", num4));
		Console.instance.Print("================================");
	}
}

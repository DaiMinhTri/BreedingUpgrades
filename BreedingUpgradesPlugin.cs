using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;

[assembly: NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
[assembly: SynchronizationMode(AdminOnlyStrictness.IfOnServer)]

namespace BreedingUpgrades;

[BepInPlugin("DMT.breedingupgrades", "BreedingUpgrades", "1.0.4")]
public class BreedingUpgradesPlugin : BaseUnityPlugin
{
	public const string PLUGIN_GUID = "DMT.breedingupgrades";

	public const string PLUGIN_NAME = "BreedingUpgrades";

	public const string PLUGIN_VERSION = "1.0.4";

	private static BreedingUpgradesPlugin instance;

	private static ManualLogSource logger;

	private Harmony harmony;

	private static ConfigEntry<bool> serverConfigLocked;

	public static ConfigEntry<bool> EnableMod { get; private set; }

	public static ConfigEntry<int> UpgradeChance { get; private set; }

	public static ConfigEntry<int> MaxStarLevel { get; private set; }

	public static ConfigEntry<bool> DebugLogging { get; private set; }

	public static ConfigEntry<bool> EnableBreedingLimit { get; private set; }

	public static Dictionary<string, ConfigEntry<int>> MaxCreaturesPerSpecies { get; private set; } = new Dictionary<string, ConfigEntry<int>>();

	private void Awake()
	{
		instance = this;
		logger = Logger;
		InitializeConfig();
		ValidateConfiguration();
		harmony = new Harmony("DMT.breedingupgrades");
		harmony.PatchAll();
		SynchronizationManager.OnConfigurationSynchronized += OnConfigSync;
		LogInfo("BreedingUpgrades v" + PLUGIN_VERSION + " loaded successfully.");
		LogInfo($"Upgrade Chance: {UpgradeChance.Value}%, Max Stars: {MaxStarLevel.Value}");
		LogInfo($"Breeding Limit: {(EnableBreedingLimit.Value ? "Enabled" : "Disabled")}");
	}

	private void OnDestroy()
	{
		harmony?.UnpatchSelf();
		SynchronizationManager.OnConfigurationSynchronized -= OnConfigSync;
	}

	private void OnConfigSync(object sender, ConfigurationSynchronizationEventArgs e)
	{
		if (e.InitialSynchronization)
		{
			LogInfo("Initial config sync received from server.");
		}
		else
		{
			LogInfo("Config sync updated by server.");
		}
		ValidateConfiguration();
	}

	private void InitializeConfig()
	{
		serverConfigLocked = Config.Bind("1. General", "Lock Configuration", true, new ConfigDescription("If enabled, configuration is locked and can only be changed by server admins."));

		EnableMod = Config.Bind("1. General", "Enable Mod", true, new ConfigDescription("Master toggle for mod functionality.", null, new ConfigurationManagerAttributes { IsAdminOnly = true }));

		UpgradeChance = Config.Bind("2. Breeding", "Upgrade Chance", 5, new ConfigDescription("Percentage chance for offspring to gain +1 star level.", new AcceptableValueRange<int>(0, 100), new ConfigurationManagerAttributes { IsAdminOnly = true }));
		UpgradeChance.SettingChanged += (_, _) => { ValidateConfiguration(); };

		MaxStarLevel = Config.Bind("2. Breeding", "Max Star Level", 2, new ConfigDescription("Maximum star level allowed for offspring.\nVanilla Valheim supports 0-2 stars (levels 1-3).\nHigher values require CLLC or similar creature level mod.\nWithout such a mod, values above 2 will be capped at 2.", new AcceptableValueRange<int>(0, 10), new ConfigurationManagerAttributes { IsAdminOnly = true }));
		MaxStarLevel.SettingChanged += (_, _) => { ValidateConfiguration(); };

		EnableBreedingLimit = Config.Bind("3. Breeding Limits", "Enable Breeding Limit", true, new ConfigDescription("Enable custom population cap per species. When enabled, breeding stops when the species limit is reached nearby.", null, new ConfigurationManagerAttributes { IsAdminOnly = true }));
		EnableBreedingLimit.SettingChanged += (_, _) => { LogInfo("Breeding limit toggled."); };

		MaxCreaturesPerSpecies["Boar"] = Config.Bind("3. Breeding Limits", "Max Boars", 5, new ConfigDescription("Max boars nearby before breeding stops. Vanilla is about 4.", new AcceptableValueRange<int>(3, 20), new ConfigurationManagerAttributes { IsAdminOnly = true }));
		MaxCreaturesPerSpecies["Wolf"] = Config.Bind("3. Breeding Limits", "Max Wolves", 5, new ConfigDescription("Max wolves nearby before breeding stops. Vanilla is about 4.", new AcceptableValueRange<int>(3, 20), new ConfigurationManagerAttributes { IsAdminOnly = true }));
		MaxCreaturesPerSpecies["Lox"] = Config.Bind("3. Breeding Limits", "Max Lox", 5, new ConfigDescription("Max lox nearby before breeding stops. Vanilla is about 4.", new AcceptableValueRange<int>(3, 20), new ConfigurationManagerAttributes { IsAdminOnly = true }));
		MaxCreaturesPerSpecies["Chicken"] = Config.Bind("3. Breeding Limits", "Max Chickens", 5, new ConfigDescription("Max chickens nearby before breeding stops. Vanilla is about 4.", new AcceptableValueRange<int>(3, 20), new ConfigurationManagerAttributes { IsAdminOnly = true }));
		MaxCreaturesPerSpecies["Moose"] = Config.Bind("3. Breeding Limits", "Max Moose", 5, new ConfigDescription("Max moose nearby before breeding stops. Vanilla is about 4.", new AcceptableValueRange<int>(3, 20), new ConfigurationManagerAttributes { IsAdminOnly = true }));
		MaxCreaturesPerSpecies["Asksvin"] = Config.Bind("3. Breeding Limits", "Max Asksvin", 5, new ConfigDescription("Max asksvin nearby before breeding stops. Vanilla is about 4.", new AcceptableValueRange<int>(3, 20), new ConfigurationManagerAttributes { IsAdminOnly = true }));

		DebugLogging = Config.Bind("4. Debug", "Enable Debug Logging", false, new ConfigDescription("Enable verbose logging for troubleshooting."));
		DebugLogging.SettingChanged += (_, _) => { LogInfo("Debug logging toggled."); };
	}

	private void ValidateConfiguration()
	{
		if (MaxStarLevel.Value > 2)
		{
			LogWarning($"MaxStarLevel is set to {MaxStarLevel.Value}. " + "This requires CLLC or a similar mod. Without it, upgrades will cap at 2 stars.");
		}
		if (UpgradeChance.Value > 50)
		{
			LogWarning($"UpgradeChance is set to {UpgradeChance.Value}%. " + "This will cause rapid star level escalation in your animal population.");
		}
	}

	public static void LogDebug(string message)
	{
		if (DebugLogging.Value)
		{
			logger.LogDebug(message);
		}
	}

	public static void LogInfo(string message)
	{
		logger.LogInfo(message);
	}

	public static void LogWarning(string message)
	{
		logger.LogWarning(message);
	}

	public static void LogError(string message)
	{
		logger.LogError(message);
	}

	public static int GetMaxCreatures(string species)
	{
		if (MaxCreaturesPerSpecies.TryGetValue(species, out var entry))
		{
			return entry.Value;
		}
		return 5;
	}
}

using UnityEngine;

namespace BreedingUpgrades.Patches;

public static class ProcreationContext
{
	public static bool IsInProcreation { get; set; }

	public static int ParentLevel { get; set; }

	public static GameObject ParentA { get; set; }

	public static GameObject ParentB { get; set; }
}

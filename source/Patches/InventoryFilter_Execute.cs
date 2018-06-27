using BattleTech.UI;
using Harmony;
using System;
using System.Collections.Generic;
using System.Text;

namespace MechEngineer
{
    [HarmonyPatch(typeof(InventoryFilter), "Execute")]
    public static class InventoryFilter_Execute
    {
        [HarmonyPrefix]
        public static bool TestPrefix(InventoryFilter __instance,
            ref IEnumerable<ListElementController_BASE> __result,
            IEnumerable<ListElementController_BASE> listData)
        {
            Control.mod.Logger.Log("FILTER PREFIXED!");
            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(InventoryFilter __instance,
            ref IEnumerable<ListElementController_BASE> __result,
            IEnumerable<ListElementController_BASE> listData)
        {
            Control.mod.Logger.Log("FILTER postfixed!");
        }
    }

    [HarmonyPatch(typeof(InventoryGearUpgradeFilter), "Execute")]
    public static class InventoryGearUpgradeFilter_Execute
    {
        [HarmonyPrefix]
        public static bool TestPrefix(InventoryGearUpgradeFilter __instance,
            ref IEnumerable<ListElementController_BASE> __result,
            IEnumerable<ListElementController_BASE> listData)
        {
            Control.mod.Logger.Log("InventoryGearUpgradeFilter FILTER PREFIXED!");
            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(InventoryGearUpgradeFilter __instance,
            ref IEnumerable<ListElementController_BASE> __result,
            IEnumerable<ListElementController_BASE> listData)
        {
            Control.mod.Logger.Log("InventoryGearUpgradeFilter FILTER postfixed!");
        }
    }

    [HarmonyPatch(typeof(InventoryWeaponsFilter), "Execute")]
    public static class InventoryWeaponsFilter_Execute
    {
        [HarmonyPrefix]
        public static bool TestPrefix(InventoryWeaponsFilter __instance,
            ref IEnumerable<ListElementController_BASE> __result,
            IEnumerable<ListElementController_BASE> listData)
        {
            Control.mod.Logger.Log("InventoryWeaponsFilter FILTER PREFIXED!");
            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(InventoryWeaponsFilter __instance,
            ref IEnumerable<ListElementController_BASE> __result,
            IEnumerable<ListElementController_BASE> listData)
        {
            Control.mod.Logger.Log("InventoryWeaponsFilter FILTER postfixed!");
        }
    }



    [HarmonyPatch(typeof(InventoryWeaponEnergyFilter), "Execute")]
    public static class InventoryWeaponEnergyFilter_Execute
    {
        [HarmonyPrefix]
        public static bool TestPrefix(InventoryWeaponEnergyFilter __instance,
            ref IEnumerable<ListElementController_BASE> __result,
            IEnumerable<ListElementController_BASE> listData)
        {
            Control.mod.Logger.Log("InventoryWeaponEnergyFilter FILTER PREFIXED!");
            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(InventoryWeaponEnergyFilter __instance,
            ref IEnumerable<ListElementController_BASE> __result,
            IEnumerable<ListElementController_BASE> listData)
        {
            Control.mod.Logger.Log("InventoryWeaponEnergyFilter FILTER postfixed!");
        }
    }
}

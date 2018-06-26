using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabInventoryWidget_ListView), "ApplyFiltering")]
    public class MechLabInventoryWidget_ListViewApplyFilter
    {
        public static void Postfix(MechLabInventoryWidget_ListView __instance)
        {
            __instance.ClearListViewFilters();
            float mechTonnageForJumpJets = -1f;

            var mechtonnage = Traverse.Create(__instance).Field("mechTonnage").GetValue<float>();

            if (mechtonnage > 0f)
            {
                mechTonnageForJumpJets = mechtonnage;
            }

            var builder = Traverse.Create(__instance).Field("inventoryFilterBuilder");
            builder.SetValue(
                builder.GetValue<FilterPipelineBuilder<ListElementController_BASE>>().ReplaceOrAddFilter(
                    new ChasisInventoryFilter(
                        Traverse.Create(__instance).Field("filteringAll").GetValue<bool>(),
                        Traverse.Create(__instance).Field("filteringWeapons").GetValue<bool>(),
                        Traverse.Create(__instance).Field("filterEnabledWeaponBallistic").GetValue<bool>(),
                        Traverse.Create(__instance).Field("filterEnabledWeaponEnergy").GetValue<bool>(),
                        Traverse.Create(__instance).Field("filterEnabledWeaponMissile").GetValue<bool>(),
                        Traverse.Create(__instance).Field("filterEnabledWeaponSmall").GetValue<bool>(),
                        Traverse.Create(__instance).Field("filteringEquipment").GetValue<bool>(),
                        Traverse.Create(__instance).Field("filterEnabledHeatsink").GetValue<bool>(),
                        Traverse.Create(__instance).Field("filterEnabledJumpjet").GetValue<bool>(),
                        mechTonnageForJumpJets,
                        Traverse.Create(__instance).Field("filterEnabledUpgrade").GetValue<bool>(),
                        Traverse.Create(__instance).Field("filteringMechParts").GetValue<bool>()
                    )
                )
            );

            var list = Traverse.Create(__instance).Field("ListView").GetValue<HBSInventoryLoopingListView>();

            list.ListViewItemFilter = builder.GetValue<FilterPipelineBuilder<ListElementController_BASE>>().ToPipeline();
            list.Refresh();
        }
    }
}
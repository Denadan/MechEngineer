using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using Harmony;
using TMPro;

namespace MechEngineer
{
    public static class UniqueController
    {


    }

//    [HarmonyPatch(typeof(MechLabLocationWidget), "ValidateAdd", new Type[] {typeof(MechComponentDef)})]
    public static class MechLabLocationWidget_ValidateAdd_Patch
    {
        public static void Postfix(MechComponentDef newComponentDef,
            MechLabLocationWidget __instance, ref bool __result, ref string ___dropErrorMessage,
            List<MechLabItemSlotElement> ___localInventory,
            int ___usedSlots,
            int ___maxSlots,
            TextMeshProUGUI ___locationName,
            MechLabPanel ___mechLab

        )
        {
            Control.mod.Logger.Log("ValidateAdd started at " + ___locationName.text);
            Control.mod.Logger.Log("Check for " + newComponentDef == null ? "NULL!" : newComponentDef.Description.Id);

            UniqueItem unique_info;
            //if item not in unique list - continue normal flow
            if (newComponentDef == null)
                return;

            if (!newComponentDef.IsUnique(out unique_info))
            {
                Control.mod.Logger.Log("Not unique!");
                return;
            }

            Control.mod.Logger.Log("is unique, freeslot");

            //if cannot place and cause not size - continue normal flow
            if (!__result && !___dropErrorMessage.EndsWith("Not enough free slots."))
                return;

            //find if have another this type item in inventory
            var n = ___localInventory.FindUniqueItem(unique_info);

            Control.mod.Logger.Log("index = " + n.ToString());
            //if no - continue normal flow(add new or show "not enough slots" message
            if (n < 0)
                return;

            Control.mod.Logger.Log("freeslot2");

            //if cannot fit new item with replacement
            if (___usedSlots - ___localInventory[n].ComponentRef.Def.InventorySize + newComponentDef.InventorySize >
                ___maxSlots)
            {
                __result = false;
                ___dropErrorMessage = string.Format("Cannot add {0} to {1}: Not enough free slots.",
                    newComponentDef.Description.Name, ___locationName.text);
                return;
            }

            Control.mod.Logger.Log("replacement");

            //do replacement 
            var dragItem = ___mechLab.DragItem;

            var old_item = ___localInventory[n];
            __instance.OnRemoveItem(old_item, true);
            ___mechLab.ForceItemDrop(old_item);
            var clear = __instance.OnAddItem(___mechLab.DragItem, true);
            if (__instance.Sim != null)
            {
                WorkOrderEntry_InstallComponent subEntry = __instance.Sim.CreateComponentInstallWorkOrder(
                    ___mechLab.baseWorkOrder.MechID,
                    dragItem.ComponentRef, __instance.loadout.Location, dragItem.MountedLocation);
                ___mechLab.baseWorkOrder.AddSubEntry(subEntry);
            }

            dragItem.MountedLocation = __instance.loadout.Location;
            ___mechLab.ClearDragItem(clear);
            __instance.RefreshHardpointData();
            ___mechLab.ValidateLoadout(false);
            __result = false;
            ___dropErrorMessage = "Item Replaced";
            Control.mod.Logger.Log("done");

        }
    }



    [HarmonyPatch(typeof(MechLabLocationWidget), "OnMechLabDrop")]
    public static class MechLabLocationWidget_OnDrop
    {
        public static bool Prefix(MechLabLocationWidget __instance, ref string ___dropErrorMessage,
            List<MechLabItemSlotElement> ___localInventory,
            int ___usedSlots,
            int ___maxSlots,
            TextMeshProUGUI ___locationName,
            MechLabPanel ___mechLab)
        {
            Control.mod.Logger.Log("Drop on " + ___locationName.text);


            if (!___mechLab.Initialized)
            {
                Control.mod.Logger.Log("not Initialized");

                return false;
            }
            if (___mechLab.DragItem == null)
            {
                Control.mod.Logger.Log("Dragged item: NULL, exit");
                return false;
            }

            var drag_item = ___mechLab.DragItem;


            if (drag_item.ComponentRef == null)
            {
                Control.mod.Logger.Log("Dropped item: NULL, exit");

                return false;
            }

            Control.mod.Logger.Log("Dropped item: " + drag_item.ComponentRef.ComponentDefID);

            UniqueItem new_item_info;
            if (!drag_item.ComponentRef.Def.IsUnique(out new_item_info))
            {
                Control.mod.Logger.Log("Item not Unique, exit");

                return true;
            }

            bool flag = __instance.ValidateAdd(drag_item.ComponentRef);

            Control.mod.Logger.Log(string.Format("Validation: {0} - {1}", flag, ___dropErrorMessage));

            if (!flag && !___dropErrorMessage.EndsWith("Not enough free slots."))
                return true;

            var n = ___localInventory.FindUniqueItem(new_item_info);

            Control.mod.Logger.Log("index = " + n.ToString());

            //if no - continue normal flow(add new or show "not enough slots" message
            if (n < 0)
                return true;

            if (___usedSlots - ___localInventory[n].ComponentRef.Def.InventorySize + drag_item.ComponentRef.Def.InventorySize >
                ___maxSlots)
            {
                return true;
            }

            var old_item = ___localInventory[n];
            __instance.OnRemoveItem(old_item, true);
            ___mechLab.ForceItemDrop(old_item);
            var clear = __instance.OnAddItem(drag_item, true);
            if (__instance.Sim != null)
            {
                WorkOrderEntry_InstallComponent subEntry = __instance.Sim.CreateComponentInstallWorkOrder(
                    ___mechLab.baseWorkOrder.MechID,
                    drag_item.ComponentRef, __instance.loadout.Location, drag_item.MountedLocation);
                ___mechLab.baseWorkOrder.AddSubEntry(subEntry);
            }

            drag_item.MountedLocation = __instance.loadout.Location;
            ___mechLab.ClearDragItem(clear);
            __instance.RefreshHardpointData();
            ___mechLab.ValidateLoadout(false);
            return false;
        }

    }
}
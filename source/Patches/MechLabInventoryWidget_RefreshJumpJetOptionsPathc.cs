using BattleTech;
using BattleTech.UI;
using Harmony;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechLabInventoryWidget), "ApplyFiltering")]
    public static class MechLabInventoryWidget_RefreshJumpJetOptions
    {
        public static void Postfix(MechLabInventoryWidget __instance, float ___mechTonnage,
            List<InventoryItemElement_NotListView> ___localInventory)
        {
            foreach (var item in ___localInventory)
            {
                MechComponentDef component = null;

                if (item.controller != null)
                    component = item.controller.componentDef;
                else if (item.ComponentRef != null)
                    component = item.ComponentRef.Def;

                if(component != null && component.IsStructure())
                {
                    var tonnage = component.GetStructureWeight();
//                    Control.mod.Logger.Log(string.Format("[{0}] found structure {1} {2}/{3}", Time.realtimeSinceStartup,
 //                       component.Description.Id, tonnage, ___mechTonnage));
                    item.gameObject.SetActive(
                        (___mechTonnage < 0 ||
                        ___mechTonnage == tonnage) && item.gameObject.activeSelf
                        );
                }
            }

        }

    }
}

using System;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    public static class MechLab
    {

        public static MechLabPanel Current
        {
            set { GlobalReference.Target = value; }
            get { return GlobalReference.Target as MechLabPanel; }
        }

        private static readonly WeakReference GlobalReference = new WeakReference(null);

        [HarmonyPatch(typeof(MechLabPanel), "LoadMech")]
        public static class MechLabPanelLoadMechPatch
        {
            public static void Postfix(MechLabPanel __instance, MechDef ___activeMechDef)
            {
                try
                {
                    Current = __instance;
                }
                catch (Exception e)
                {
                    Control.mod.Logger.LogError(e);
                }

                ReservedSlots.CurrentMechDef = ___activeMechDef;
                ReservedSlots.RefreshData(___activeMechDef);
            }
        }

        [HarmonyPatch(typeof(MechLabPanel), "ExitMechLab")]
        public static class MechLabPanelExitMechLabPatch
        {
            public static void Postfix(MechLabPanel __instance)
            {
                try
                {
                    Current = null;
                }
                catch (Exception e)
                {
                    Control.mod.Logger.LogError(e);
                }
                ReservedSlots.CurrentMechDef = null;

            }
        }

        [HarmonyPatch(typeof(MechLabPanel), "ValidateLoadout")]
        public static class MechLabPanelValidateLoadoutPatch
        {
            public static void Postfix(MechLabPanel __instance, MechDef ___activeMechDef)
            {
                ReservedSlots.RefreshData(___activeMechDef);

            }
        }

    }
}
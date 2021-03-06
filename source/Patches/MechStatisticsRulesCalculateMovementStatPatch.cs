﻿using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechStatisticsRules), "CalculateMovementStat")]
    public static class MechStatisticsRulesCalculateMovementStatPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                .MethodReplacer(
                    AccessTools.Method(typeof(MovementCapabilitiesDef), "get_MaxSprintDistance"),
                    AccessTools.Method(typeof(MechStatisticsRulesCalculateMovementStatPatch), "OverrideMaxSprintDistance")
                    )
                .MethodReplacer(
                    AccessTools.Method(typeof(MechDef), "get_Inventory"),
                    AccessTools.Method(typeof(MechStatisticsRulesCalculateMovementStatPatch), "OverrideInventory")
                    );
        }

        private static MechDef def;

        public static void Prefix(MechDef mechDef)
        {
            def = mechDef;
        }

        public static void Postfix()
        {
            def = null;
        }

        public static float OverrideMaxSprintDistance(this MovementCapabilitiesDef @this)
        {
            try
            {

                float walkSpeed = 0, runSpeed = 0, TTWalkSpeed = 0;
                EngineMisc.CalculateMovementStat(def, ref walkSpeed, ref runSpeed, ref TTWalkSpeed);
                return runSpeed;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
            }
            return @this.MaxSprintDistance;
        }
        
        // disable jump jet calculations
        private static readonly MechComponentRef[] Empty = { };
        public static MechComponentRef[] OverrideInventory(this MechDef @this)
        {
            return Empty;
        }
    }
}



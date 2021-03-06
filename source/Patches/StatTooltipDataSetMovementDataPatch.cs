﻿using System;
using System.Collections.Generic;
using BattleTech;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(StatTooltipData), "SetMovementData")]
    public static class StatTooltipDataSetMovementDataPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return instructions
                .MethodReplacer(
                    AccessTools.Method(typeof(MovementCapabilitiesDef), "get_MaxWalkDistance"),
                    AccessTools.Method(typeof(StatTooltipDataSetMovementDataPatch), "OverrideMaxWalkDistance")
                )
                .MethodReplacer(
                    AccessTools.Method(typeof(MovementCapabilitiesDef), "get_MaxSprintDistance"),
                    AccessTools.Method(typeof(StatTooltipDataSetMovementDataPatch), "OverrideMaxSprintDistance")
                );
        }

        private static MechDef mechDef;
        private static float walkSpeed;
        private static float runSpeed;
        private static float TTWalkSpeed;

        public static void Prefix(MechDef def)
        {
            mechDef = def;
        }

        public static void Postfix(StatTooltipData __instance)
        {
            mechDef = null;
            __instance.dataList.Add("TT Walk MP", string.Format("{0}", TTWalkSpeed));
        }

        public static float OverrideMaxWalkDistance(this MovementCapabilitiesDef @this)
        {
            try
            {
                EngineMisc.CalculateMovementStat(mechDef, ref walkSpeed, ref runSpeed, ref TTWalkSpeed);
                return walkSpeed;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
                return @this.MaxWalkDistance;
            }
        }

        public static float OverrideMaxSprintDistance(this MovementCapabilitiesDef @this)
        {
            try
            {
                EngineMisc.CalculateMovementStat(mechDef, ref walkSpeed, ref runSpeed, ref TTWalkSpeed);
                return runSpeed;
            }
            catch (Exception e)
            {
                Control.mod.Logger.LogError(e);
                return @this.MaxSprintDistance;
            }
        }
    }
}
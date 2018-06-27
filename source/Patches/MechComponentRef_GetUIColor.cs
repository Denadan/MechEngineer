using BattleTech;
using BattleTech.UI;
using Harmony;

namespace MechEngineer
{
    [HarmonyPatch(typeof(MechComponentRef), "GetUIColor")]
    public static class MechComponentRef_GetUIColor
    {

        [HarmonyPostfix]
        public static void Postfix(MechComponentRef __instance,
            ref UIColor __result,
            MechComponentRef componentRef)
        {
            if (componentRef == null || componentRef.Def == null)
                return;

            if (componentRef.Def.IsArmor())
                __result = UIColor.GoldHalf;
            else if (componentRef.Def.IsStructure())
                __result = UIColor.OrangeHalf;
        }
    }
}

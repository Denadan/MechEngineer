using BattleTech;
using BattleTech.UI;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MechEngineer
{
    public static class Armor
    {
        internal static void ValidationRulesCheck(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {

        }

        internal static bool ProcessWeaponHit(MechComponent mechComponent, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            return !mechComponent.componentDef.IsArmor();
        }

        internal static void AddArmorIfPossible(MechDef mechDef)
        {
            if (!Control.settings.AutoFixArmor)
            {
                return;
            }

            if (mechDef.Inventory.Any(x => x.Def != null && x.Def.IsArmor()))
            {
                return;
            }

            var componentRefs = new List<MechComponentRef>(mechDef.Inventory);

            var componentRef = new MechComponentRef(Control.settings.AutoFixArmorDef, null, ComponentType.Upgrade, ChassisLocations.CenterTorso);
            componentRefs.Add(componentRef);

            mechDef.SetInventory(componentRefs.ToArray());
        }

        internal static void AdjustTooltip(TooltipPrefab_EquipmentAdapter tooltip, MechLabPanel panel, MechComponentDef mechComponentDef)
        {
            if (mechComponentDef.IsArmor())
            {
                var mechDef = panel.activeMechDef;
                var tonnage = Tonnage(mechDef);
                tooltip.tonnageText.text = string.Format("{0:F2}", tonnage);
            }
        }



        public static float Tonnage(MechDef mechDef)
        {

            var num = 0f;
            num += mechDef.Head.AssignedArmor;
            num += mechDef.CenterTorso.AssignedArmor;
            num += mechDef.CenterTorso.AssignedRearArmor;
            num += mechDef.LeftTorso.AssignedArmor;
            num += mechDef.LeftTorso.AssignedRearArmor;
            num += mechDef.RightTorso.AssignedArmor;
            num += mechDef.RightTorso.AssignedRearArmor;
            num += mechDef.LeftArm.AssignedArmor;
            num += mechDef.RightArm.AssignedArmor;
            num += mechDef.LeftLeg.AssignedArmor;
            num += mechDef.RightLeg.AssignedArmor;

            num /= (UnityGameInstance.BattleTechGame.MechStatisticsConstants.ARMOR_PER_TENTH_TON * 10f);

            var armor = mechDef.Inventory.Select(c => c.Def).FirstOrDefault(c => c.IsArmor());
            if (armor == null)
                return num;

            float factor = armor.GetArmorWeightMod();
            if (factor == 1f)
                return num;


            return (num / factor).RoundStandard();
        }

        public static float TonnageSave(MechDef mechDef)
        {
            var armor = mechDef.Inventory.Select(c => c.Def).FirstOrDefault(c => c.IsArmor());

            if (armor == null)
                return 0f;

            float factor = armor.GetArmorWeightMod();
            if (factor == 1f)
                return 0f;

            var num = 0f;
            num += mechDef.Head.AssignedArmor;
            num += mechDef.CenterTorso.AssignedArmor;
            num += mechDef.CenterTorso.AssignedRearArmor;
            num += mechDef.LeftTorso.AssignedArmor;
            num += mechDef.LeftTorso.AssignedRearArmor;
            num += mechDef.RightTorso.AssignedArmor;
            num += mechDef.RightTorso.AssignedRearArmor;
            num += mechDef.LeftArm.AssignedArmor;
            num += mechDef.RightArm.AssignedArmor;
            num += mechDef.LeftLeg.AssignedArmor;
            num += mechDef.RightLeg.AssignedArmor;

            num /= (UnityGameInstance.BattleTechGame.MechStatisticsConstants.ARMOR_PER_TENTH_TON * 10f);

            return (num - num / factor).RoundStandard();
        }

    }
}

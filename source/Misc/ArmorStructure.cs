using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    internal static class ArmorStructure
    {
        internal static void ValidationRulesCheck(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            var structure = mechDef.Inventory.Select(i => i.Def).Where(i => i.IsStructure()).ToArray();
            var armor = mechDef.Inventory.Select(i => i.Def).Where(i => i.IsArmor()).ToArray();


            var slot_needs = mechDef.GetAdditionalSlots();

            if (structure.Length == 0)
            {
                errorMessages[MechValidationType.InvalidHardpoints].Add("Structure Missing!");
            }
            else if (structure.Length > 1)
            {
                errorMessages[MechValidationType.InvalidHardpoints].Add("Invalid structure allocation, left only one");
            }
            else if(mechDef.Chassis.Tonnage != structure[0].GetStructureWeight())
            {
                errorMessages[MechValidationType.InvalidHardpoints].Add(
                    string.Format("Structure weight missmatch. Replace {0}t structure with {1}t",
                    structure[0].GetStructureWeight(), mechDef.Chassis.Tonnage));
            }

            if (armor.Length == 0)
            {
                errorMessages[MechValidationType.InvalidHardpoints].Add("Armor Missing!");
            }
            else if (armor.Length > 1)
            {
                errorMessages[MechValidationType.InvalidHardpoints].Add("Mech cannot use more then one type of armor");
            }


            var slots = mechDef.Inventory.Select(i => i.Def).Count(i => i.IsFiller());

            if(slots != slot_needs)
            {
                if(slots > slot_needs)
                    errorMessages[MechValidationType.InvalidInventorySlots].Add(string.Format("Too many reserved slots, remove {0}", slot_needs - slots));
                else
                    errorMessages[MechValidationType.InvalidInventorySlots].Add(string.Format("Not enough reserved slots, add {0}", slot_needs - slots));
            }
        }

        internal static bool ProcessWeaponHit(MechComponent mechComponent, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            if (mechComponent.componentDef.IsArmor()
                || mechComponent.componentDef.IsStructure() 
                || mechComponent.componentDef.IsFiller())
            {
                return false;
            }

            return true;
        }

        
        internal static void AdjustTooltip(TooltipPrefab_EquipmentAdapter tooltip, MechLabPanel panel, MechComponentDef mechComponentDef)
        {
            var mechDef = panel.activeMechDef;
            if (mechComponentDef.IsArmor())
            {
                var tonnage = ArmorTonnage(mechDef);
                tooltip.tonnageText.text = string.Format("{0:F2}", tonnage);
            }
            else if (mechComponentDef.IsFiller())
            {
                var slot_needs = mechDef.GetAdditionalSlots();
                var slots = mechDef.Inventory.Select(i => i.Def).Count(i => i.IsFiller());
                tooltip.bonusesText.text = string.Format("Reserved slots: {0} / {1}", slots, slot_needs);
            }
        }


        public static float ArmorTonnage(MechDef mechDef)
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

        public static float ArmorTonnageSave(MechDef mechDef)
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
using BattleTech;
using BattleTech.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MechEngineer
{
    public static class Structure
    {
        internal static void ValidationRulesCheck(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            var structure = mechDef.Inventory.Select(i => i.Def).Where(i => i.IsStructure()).ToArray();
            if(structure.Length == 0)
                return;

            if (mechDef.Chassis.Tonnage != structure[0].GetStructureWeight())
            {
                errorMessages[MechValidationType.InvalidHardpoints].Add(
                    string.Format("Structure weight missmatch. Replace {0}t structure with {1}t",
                    structure[0].GetStructureWeight(), mechDef.Chassis.Tonnage));
            }
        }

        internal static void AddStructureIfPossible(MechDef mechDef)
        {
            if (!Control.settings.AutoFixStructure)
            {
                return;
            }

            if (mechDef.Inventory.Any(x => x.Def != null && x.Def.IsStructure()))
            {
                return;
            }

            var componentRefs = new List<MechComponentRef>(mechDef.Inventory);

            var struct_def = Control.settings.AutoFixStructureDef + mechDef.Chassis.Tonnage;

            var componentRef = new MechComponentRef(struct_def, null, ComponentType.Upgrade, ChassisLocations.CenterTorso);
            componentRefs.Add(componentRef);

            mechDef.SetInventory(componentRefs.ToArray());
        }


        internal static bool ProcessWeaponHit(MechComponent mechComponent, WeaponHitInfo hitInfo, ComponentDamageLevel damageLevel, bool applyEffects)
        {
            return !mechComponent.componentDef.IsStructure();
        }


        internal static void AdjustTooltip(TooltipPrefab_EquipmentAdapter tooltip, MechLabPanel panel, MechComponentDef mechComponentDef)
        {
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.UI;

namespace MechEngineer
{
    public partial class MechEngineerSettings
    {
        public UniqueItem[] Uniques =
        {
            new UniqueItem {ItemPrefix = "emod_armor_", ReplaceTag = "armor"},
            new UniqueItem {ItemPrefix = "emod_structure_", ReplaceTag = "structure"},
            new UniqueItem {ItemPrefix = "emod_engine_", ReplaceTag = "engine"},
            new UniqueItem {ItemPrefix = "emod_engineslots_std_center", ReplaceTag = "engine_ct"},
            new UniqueItem {ItemPrefix = "emod_engineslots_xl_center", ReplaceTag = "engine_ct"},
            new UniqueItem {ItemPrefix = "emod_engineslots_xl_left", ReplaceTag = "engine_lt"},
            new UniqueItem {ItemPrefix = "emod_engineslots_xl_right", ReplaceTag = "engine_rt"},

        };
    }

    public class UniqueItem
    {
        public string ItemPrefix;
        public string ReplaceTag;
        public bool Required = true;
        public bool CanRemove = true;
    }

    internal static partial class Extensions
    {
        public static bool IsUnique(this MechComponentDef componentDef)
        {
            if (componentDef == null)
                return false;

            return Control.settings.Uniques.Any(i => componentDef.Description.Id.StartsWith(i.ItemPrefix));
        }

        public static bool IsUnique(this MechComponentDef componentDef, out UniqueItem uniqueinfo)
        {
            uniqueinfo = null;

            if (componentDef == null)
                return false;

            uniqueinfo = Control.settings.Uniques.FirstOrDefault(i => componentDef.Description.Id.StartsWith(i.ItemPrefix));
            return uniqueinfo != null;
        }

        public static int FindUniqueItem(this List<MechLabItemSlotElement> inventory, UniqueItem item)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                UniqueItem temp;
                if (inventory[i].ComponentRef.Def.IsUnique(out temp) && temp.ReplaceTag == item.ReplaceTag)
                    return i;
            }

            return -1;
        }
    }

    
}
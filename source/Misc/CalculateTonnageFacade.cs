using System.Linq;
using BattleTech;

namespace MechEngineer
{
    internal static class CalculateTonnageFacade
    {
        internal static float AdditionalTonnage(MechDef mechDef)
        {
            float tonnage = 0;
            tonnage += EngineMisc.TonnageChanges(mechDef);
            tonnage -= Armor.TonnageSave(mechDef);
            return tonnage;
        }
    }
}
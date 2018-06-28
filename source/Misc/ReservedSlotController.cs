using BattleTech;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Harmony;
using UnityEngine;
using BattleTech.UI;

namespace MechEngineer
{
    public class ReservedSlots
    {
        private static MechDef mech_def;

        public static MechDef CurrentMechDef
        {
            get { return mech_def; }
            set
            {
                mech_def = value;
                if(mech_def == null)
                    foreach (var pair in locations)
                    {
                        pair.Value.Clear();
                    }
            }
        }

        private static Dictionary<ChassisLocations, LocationInfo> locations;

        internal static void ValidationRulesCheck(MechDef mechDef, ref Dictionary<MechValidationType, List<string>> errorMessages)
        {
            if (locations == null)
                return;

            if (CurrentMechDef == null || mechDef != CurrentMechDef)
            {
                var total = 60; //!TODO get data from mechdef!
                var used = mechDef.Inventory.Where(i => i.Def != null).Sum(i => i.Def.InventorySize);
                var need = mechDef.GetReservedSlots();

                if (need > total - used)
                    errorMessages[MechValidationType.InvalidInventorySlots].Add(string.Format("RESERVED: {0} slots requred, you have only {1}", need, total - used));
            }
            else
            {
                var total = locations.Sum(i => i.Value.MaxSlots);
                var used = locations.Sum(i => i.Value.UsedSlots);
                var need = CurrentMechDef.GetReservedSlots();

                if (need > total - used)
                    errorMessages[MechValidationType.InvalidInventorySlots].Add(string.Format("RESERVED: {0} slots requred, you have only {1}", need, total - used));
            }
        }

        public static void RegisterLocation(LocationInfo location)
        {
            if (locations == null)
                locations = new Dictionary<ChassisLocations, LocationInfo>();

            if (location == null)
                return;

            Control.mod.Logger.Log(string.Format("{0} Registrer, {1}/{2} slots", location.ChassisLocation, location.UsedSlots, location.MaxSlots));

            LocationInfo temp;
            if (locations.TryGetValue(location.ChassisLocation, out temp))
                locations[location.ChassisLocation] = location;
            else
                locations.Add(location.ChassisLocation, location);
        }

        public static void RefreshData(MechDef def)
        {
            if (CurrentMechDef == null)
                return;

            Control.mod.Logger.Log("refresh required");

            var total = locations.Sum(i => i.Value.MaxSlots);
            var used = locations.Sum(i => i.Value.UsedSlots);
            var need = def.GetReservedSlots();
            var slots = need;

            Control.mod.Logger.Log(string.Format("Refresh slots total:{0} used:{1} free:{2} need:{3}", total, used, total - used, need));


            foreach (var pair in locations)
            {
                slots = pair.Value.Refresh(slots, need <= total - used);
            }
        }
    }

    public class LocationInfo
    {
        public ChassisLocations ChassisLocation { get { return location.Location; } }
        public int MaxSlots { get; private set; }
        public int UsedSlots { get { return used_slots.GetValue<int>(); } }


        private LocationLoadoutDef location;
        private List<Image> FillerImages;
        private MechLabLocationWidget loadout;
        private Traverse  used_slots;

        public LocationInfo(MechLabLocationWidget widget, List<Image> images, LocationLoadoutDef location)
        {
            this.location = location;
            this.FillerImages = images;
            this.loadout = widget;

            var traverse = Traverse.Create(loadout);
            MaxSlots = traverse.Field("maxSlots").GetValue<int>();
            used_slots = traverse.Field("usedSlots");
        }

        public int Refresh(int slots, bool fit)
        {
            int used = UsedSlots;
            for(int i =0; i<MaxSlots; i++)
            {
                if (i < used)
                    FillerImages[i].gameObject.SetActive(false);
                else if(i - used < slots)
                {
                    FillerImages[i].gameObject.SetActive(true);
                    FillerImages[i].color = fit ? Control.settings.FitColor : Control.settings.UnFitColor;
                }
                else
                    FillerImages[i].gameObject.SetActive(false);
            }

            return slots - MaxSlots + used;
        }

        public void Clear()
        {
            foreach (var fillerImage in FillerImages)
            {
                GameObject.Destroy(fillerImage);
            }
        }
    }
}

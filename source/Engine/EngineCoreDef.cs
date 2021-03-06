﻿using System.Text.RegularExpressions;
using BattleTech;

namespace MechEngineer
{
    internal class EngineCoreDef
    {
        internal readonly int Rating;
        internal readonly MechComponentDef Def;

        private static readonly Regex EngineNameRegex = new Regex(@"^emod_engine_(\d+)$", RegexOptions.Compiled);

        internal EngineCoreDef(MechComponentDef componentDef)
        {
            var id = componentDef.Description.Id;
            var match = EngineNameRegex.Match(id);
            Rating = int.Parse(match.Groups[1].Value);
            Def = componentDef;

            Control.calc.CalcHeatSinks(this, out MinHeatSinks, out MaxHeatSinks);
        }

        public override string ToString()
        {
            return Def.Description.Id + " Rating=" + Rating;
        }

        internal int MinHeatSinks, MaxHeatSinks;

        internal int MaxAdditionalHeatSinks
        {
            get { return MaxHeatSinks - MinHeatSinks; }
        }

        internal float GyroTonnage
        {
            get { return Control.calc.CalcGyroWeight(this); }
        }

        public float StandardEngineTonnage
        {
            get { return Def.Tonnage - GyroTonnage; }
        }
    }
}
﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace PrisonLabor
{
    internal class Alert_StarvingPrisoners : Alert
    {
        public Alert_StarvingPrisoners()
        {
            defaultLabel = "Prisoners are starving";
            defaultExplanation = "Work in progress";
        }

        private IEnumerable<Pawn> StarvingPrisoners
        {
            get
            {
                var maps = Find.Maps;
                for (var i = 0; i < maps.Count; i++)
                    foreach (var pawn in maps[i].mapPawns.AllPawns)
                        if (PrisonLaborUtility.LaborEnabled(pawn) && PrisonLaborUtility.WorkTime(pawn) &&
                            (!PrisonLaborPrefs.EnableMotivationMechanics ||
                             !pawn.needs.TryGetNeed<Need_Motivation>().IsLazy) && pawn.timetable != null &&
                            pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Anything &&
                            pawn.needs.food.Starving)
                            yield return pawn;
            }
        }

        public override string GetExplanation()
        {
            var stringBuilder = new StringBuilder();
            foreach (var current in StarvingPrisoners)
                stringBuilder.AppendLine("    " + current.NameStringShort);
            return string.Format("Those prisoners are starving and won't work:\n\n{0}", stringBuilder);
        }

        public override AlertReport GetReport()
        {
            if (!PrisonLaborPrefs.DisableMod)
                return AlertReport.CulpritIs(StarvingPrisoners.FirstOrDefault());
            return false;
        }
    }
}
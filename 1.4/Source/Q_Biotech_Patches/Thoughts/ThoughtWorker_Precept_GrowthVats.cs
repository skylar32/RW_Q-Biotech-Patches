using Verse;
using RimWorld;
using UnityEngine;
using System.Linq;

namespace Q_Biotech_Patches
{
    public class ThoughtWorker_Precept_ChildInGrowthVat : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (p.relations.ChildrenCount <= 0) return false;

            int numChildrenInVats = this.ChildrenInGrowthVatCount(p);
            Log.Message($"children in growth vats: {numChildrenInVats}");
            return numChildrenInVats > 0
                ? ThoughtState.ActiveAtStage(Mathf.Min(2, numChildrenInVats - 1))
                : ThoughtState.Inactive;
        }

        protected int ChildrenInGrowthVatCount(Pawn pawn)
        {
            int num = 0;
            if (pawn.relations.ChildrenCount > 0)
            {
                foreach (Pawn child in pawn.relations.Children)
                {
                    if (!child.DevelopmentalStage.Adult() && child.Faction == pawn.Faction && !child.Dead &&
                        child.ParentHolder != null && child.ParentHolder is Building_GrowthVat)
                        ++num;
                }
            }

            return num;
        }
    }

    public class ThoughtWorker_Precept_AllChildrenInGrowthVat : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (p.relations.ChildrenCount <= 0)
                return false;

            int numChildrenInVats = 0,
                numChildrenTotal = 0;
            foreach (var child in p.relations.Children)
            {
                if (!child.DevelopmentalStage.Adult() && !child.Dead && child.ParentHolder != null &&
                    child.Faction == p.Faction)
                {
                    ++numChildrenTotal;
                    if (child.ParentHolder is Building_GrowthVat)
                        ++numChildrenInVats;
                }
            }

            return numChildrenInVats == numChildrenTotal
                ? ThoughtState.ActiveAtStage(Mathf.Min(1, numChildrenInVats - 1))
                : ThoughtState.Inactive;
        }
    }

    public class ThoughtWorker_Precept_ChildNotInGrowthVat : ThoughtWorker_Precept_ChildInGrowthVat
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (p.relations.ChildrenCount <= 0)
                return false;

            var numChildrenOutsideVats =
                (from child in p.relations.Children
                    where !child.DevelopmentalStage.Adult()
                    where child.Faction == p.Faction
                    where !child.Dead
                    select child).Count() - this.ChildrenInGrowthVatCount(p);
            Log.Message($"children outside growth vats: {numChildrenOutsideVats}");
            return numChildrenOutsideVats > 0
                ? ThoughtState.ActiveAtStage(Mathf.Min(2, numChildrenOutsideVats - 1))
                : ThoughtState.Inactive;
        }
    }

    public class ThoughtWorker_Precept_HasGrowthVats : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (p.Faction == null || p.IsSlave)
                return false;
            foreach (var map in Find.Maps)
            {
                foreach (var growthVat in map.listerThings.ThingsOfDef(DefDatabase<ThingDef>.GetNamed("GrowthVat")))
                {
                    if (growthVat.Faction == p.Faction)
                        return true;
                }
            }

            return false;
        }
    }

    public class ThoughtWorker_Precept_HasNoGrowthVats : ThoughtWorker_Precept_HasGrowthVats
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            return base.ShouldHaveThought(p).Active ? ThoughtState.Inactive : ThoughtState.ActiveDefault;
        }
    }
}
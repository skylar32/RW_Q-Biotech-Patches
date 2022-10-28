using System.Linq;
using Verse;
using RimWorld;
using UnityEngine;


namespace Q_Biotech_Patches
{
    public class ThoughtWorker_Precept_XenoDiversity : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            if (p.Faction == null || !p.IsColonist)
                return false;

            int numTotal = 0,
                numXeno = 0;
            foreach (var otherPawn in p.Map.mapPawns.SpawnedPawnsInFaction(p.Faction).Where(otherPawn =>
                         !otherPawn.IsQuestLodger() && otherPawn.RaceProps.Humanlike && !otherPawn.IsSlave &&
                         !otherPawn.IsPrisoner))
            {
                numTotal++;
                if (otherPawn != p && otherPawn.genes.XenotypeLabel != p.genes.XenotypeLabel)
                    numXeno++;
            }

            return numXeno == 0
                ? ThoughtState.Inactive
                : ThoughtState.ActiveAtStage(Mathf.RoundToInt((float)numXeno / (float)(numTotal - 1) *
                                                              (float)(def.stages.Count - 1)));
        }
    }

    public class ThoughtWorker_Precept_XenoDiversity_Social : ThoughtWorker_Precept_Social
    {
        protected override ThoughtState ShouldHaveThought(Pawn p, Pawn otherPawn) =>
            (ThoughtState)(p.Faction == otherPawn.Faction && p.genes.XenotypeLabel != otherPawn.genes.XenotypeLabel);
    }

    public class ThoughtWorker_Precept_XenoDiversity_Uniform : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            int num = 0;
            foreach (var otherPawn in p.Map.mapPawns.SpawnedPawnsInFaction(p.Faction))
            {
                if (otherPawn != p && otherPawn.RaceProps.Humanlike && !otherPawn.IsSlave && !otherPawn.IsPrisoner &&
                    !otherPawn.IsQuestLodger())
                {
                    if (otherPawn.genes.XenotypeLabel != p.genes.XenotypeLabel)
                        return false;
                    ++num;
                }
            }
            
            return (ThoughtState)(num > 0);
        }
    }
}
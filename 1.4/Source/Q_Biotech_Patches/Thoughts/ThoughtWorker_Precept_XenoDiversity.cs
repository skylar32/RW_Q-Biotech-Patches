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
}
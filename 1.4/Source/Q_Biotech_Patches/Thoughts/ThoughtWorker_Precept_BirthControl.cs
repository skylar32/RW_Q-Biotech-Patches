using RimWorld;
using Verse;

namespace Q_Biotech_Patches
{
    public class ThoughtWorker_Precept_BirthControl : ThoughtWorker_Precept
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            foreach (var hediff in p.health.hediffSet.hediffs)
            {
                if (!hediff.def.tags.NullOrEmpty<string>() && (hediff.def.tags.Contains("Sterilized") || hediff.def.tags.Contains("ReversibleSterilized")))
                    return true;
            }

            return false;
        }
    }
    
    public class ThoughtWorker_Precept_NoBirthControl : ThoughtWorker_Precept_BirthControl
    {
        protected override ThoughtState ShouldHaveThought(Pawn p)
        {
            return !base.ShouldHaveThought(p).Active;
        }
    }
}
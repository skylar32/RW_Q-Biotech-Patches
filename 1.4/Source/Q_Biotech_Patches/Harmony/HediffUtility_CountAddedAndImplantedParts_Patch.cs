using HarmonyLib;
using RimWorld;
using Verse;

namespace Q_Biotech_Patches
{
    [HarmonyPatch(typeof(HediffUtility))]
    [HarmonyPatch("CountAddedAndImplantedParts")]
    public static class HediffUtility_CountAddedAndImplantedParts_Patch
    {
        [HarmonyPostfix]
        public static void CountAddedAndImplantedParts(ref int __result, HediffSet hs)
        {
            if (hs.pawn.genes.Xenogenes.Any())
                ++__result;
        }
    }
}
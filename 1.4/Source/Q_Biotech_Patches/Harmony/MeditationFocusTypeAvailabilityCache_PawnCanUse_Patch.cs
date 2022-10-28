using System;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Q_Biotech_Patches
{
    [HarmonyPatch(typeof(MeditationFocusTypeAvailabilityCache))]
    [HarmonyPatch("PawnCanUseInt", MethodType.Normal)]
    public class MeditationFocusTypeAvailabilityCache_PawnCanUseInt_Patch
    {
        [HarmonyPostfix]
        public static void PawnCanUse(ref bool __result, Pawn p, MeditationFocusDef type)
        {
            var requiresTribal = (from backstory in type.requiredBackstoriesAny
                where backstory.categoryName == "Tribal" select backstory).Any();
            if (requiresTribal && !__result && p.story.Childhood == DefDatabase<BackstoryDef>.GetNamed("TribeChild19"))
                __result = true;
        }
    }

    [HarmonyPatch(typeof(Pawn_AgeTracker))]
    [HarmonyPatch("BirthdayBiological")]
    public class Pawn_AgeTracker_BirthdayBiological_Patch
    {
        [HarmonyPostfix]
        public static void BirthdayBiological(Pawn_AgeTracker __instance, Pawn ___pawn, int birthdayAge)
        {
            if ( (double) birthdayAge == (double) __instance.AdultMinAge)
                MeditationFocusTypeAvailabilityCache.ClearFor(___pawn);
        }
    }
}
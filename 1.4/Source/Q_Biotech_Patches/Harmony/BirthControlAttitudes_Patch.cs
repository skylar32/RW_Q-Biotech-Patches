using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;

namespace Q_Biotech_Patches
{
    [HarmonyPatch(typeof(HediffComp_PregnantHuman))]
    [HarmonyPatch("SetupPregnancyAttitude", MethodType.Normal)]
    public class HediffComp_PregnantHuman_SetupPregnancyAttitude_Patch
    {
        [HarmonyPrefix]
        public static bool SetupPregnancyAttitude(
            HediffComp_PregnantHuman __instance,
            ref PregnancyAttitude? ___pregnancyAttitude
            )
        {
            if (__instance.Pawn.ideo.Ideo.HasPrecept(DefDatabase<PreceptDef>.GetNamed("BirthControl_Abhorrent")))
            {
                ___pregnancyAttitude = new PregnancyAttitude?(PregnancyAttitude.Positive);
                return true;
            }
            else if (__instance.Pawn.ideo.Ideo.HasPrecept(DefDatabase<PreceptDef>.GetNamed("BirthControl_Preferred")))
            {
                ___pregnancyAttitude = new PregnancyAttitude?(PregnancyAttitude.Negative);
                return true;
            }

            return false;
        }
    }
    
    [HarmonyPatch(typeof(ThoughtWorker_PregnancyAttitude))]
    [HarmonyPatch("CurrentStateInternal")]
    public static class ThoughtWorker_PregnancyAttitude_CurrentStateInternal_Patch
    {
        [HarmonyPostfix]
        public static void CurrentStateInternal(ref ThoughtState __result, Pawn p)
        {
            if (__result.Active && p.ideo.Ideo.HasPrecept(DefDatabase<PreceptDef>.GetNamed("BirthControl_Abhorrent")))
            {
                __result = ThoughtState.ActiveAtStage(2);
            }
        }
    }
    
    [HarmonyPatch(typeof(PregnancyUtility))]
    [HarmonyPatch("TryTerminatePregnancy")]
    public static class PregnancyUtility_TryTerminatePregnancy_Patch
    {
        [HarmonyPrefix]
        public static bool TryTerminatePregnancy_Prefix(ref int __state, Pawn pawn)
        {
            Hediff pregnancyHediff = PregnancyUtility.GetPregnancyHediff(pawn);
            if (pregnancyHediff != null)
                __state = pregnancyHediff.CurStageIndex;
            return true;
        }
        
        [HarmonyPostfix]
        public static void TryTerminatePregnancy_Postfix(ref int __state, bool __result, Pawn pawn)
        {
            if (__result)
            {
                if (pawn.needs?.mood?.thoughts?.memories != null &&
                    pawn.ideo.Ideo.HasPrecept(DefDatabase<PreceptDef>.GetNamed("BirthControl_Abhorrent")))
                {
                    pawn.needs.mood.thoughts.memories.RemoveMemoriesOfDef(ThoughtDefOf.PregnancyTerminated); // don't stack these
                    
                    Thought_Memory newThought = ThoughtMaker.MakeThought(DefDatabase<ThoughtDef>.GetNamed("PregnancyTerminated_Abhorrent"), __state);
                    pawn.needs.mood.thoughts.memories.TryGainMemory(newThought);
                }
            }
        }
    }
    
    [HarmonyPatch(typeof(Recipe_TerminatePregnancy))]
    [HarmonyPatch("ApplyOnPawn")]
    public static class Recipe_TerminatePregnancy_ApplyOnPawn_Patch
    {
        [HarmonyPostfix]
        public static void ApplyOnPawn(Pawn pawn, Pawn billDoer)
        {
            Find.HistoryEventsManager.RecordEvent(new HistoryEvent(DefDatabase<HistoryEventDef>.GetNamed("PregnancyTerminated"), new SignalArgs(pawn.Named(HistoryEventArgsNames.Doer))));
        }
    }
    
    [HarmonyPatch(typeof(Recipe_ImplantIUD))]
    [HarmonyPatch("ApplyOnPawn")]
    public static class Recipe_ImplantIUD_ApplyOnPawn_Prefix
    {
        [HarmonyPrefix]
        public static bool ApplyOnPawn_Prefix(ref bool __state, Pawn pawn)
        {
            Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.PregnantHuman);
            if (firstHediffOfDef != null && firstHediffOfDef.CurStageIndex == 0)
                __state = true; // will require pregnancy termination
            else
                __state = false;
            
            return true;
        }
        
        [HarmonyPostfix]
        public static void Recipe_ImplantIUD_ApplyOnPawn_Postfix(ref bool __state, Pawn pawn, Pawn billDoer)
        {
            Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.PregnantHuman);
            if (__state && firstHediffOfDef == null) // pregnancy termination was required and occurred
            {
                Find.HistoryEventsManager.RecordEvent(new HistoryEvent(DefDatabase<HistoryEventDef>.GetNamed("PregnancyTerminated"), new SignalArgs(pawn.Named(HistoryEventArgsNames.Doer))));
            }
        }
    }
}
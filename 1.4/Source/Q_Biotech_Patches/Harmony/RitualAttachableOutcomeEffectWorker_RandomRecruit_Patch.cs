using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using RimWorld.QuestGen;
using RimWorld;
using Verse;

namespace Q_Biotech_Patches
{
    [HarmonyPatch(typeof(RitualAttachableOutcomeEffectWorker_RandomRecruit))]
    [HarmonyPatch("Apply", MethodType.Normal)]
    public class RitualAttachableOutcomeEffectWorker_RandomRecruit_Patch
    {
        [HarmonyPrefix]
        public static bool Apply(
            RitualAttachableOutcomeEffectWorker_RandomRecruit __instance,
            Dictionary<Pawn, int> totalPresence,
            LordJob_Ritual jobRitual,
            OutcomeChance outcome,
            out string extraOutcomeDesc,
            ref LookTargets letterLookTargets)
        {
            if (Rand.Chance(0.5f))
            {
                var xenotypeOptions = (from pawn in totalPresence.Keys.ToList() select pawn.genes.Xenotype);
                XenotypeDef chosenXenotype = xenotypeOptions.RandomElement();
                
                Slate vars = new Slate();
                vars.Set<Map>("map", jobRitual.Map);
                vars.Set<PawnGenerationRequest>("overridePawnGenParams", new PawnGenerationRequest(PawnKindDefOf.Villager, forceGenerateNewPawn: true, colonistRelationChanceFactor: 20f, fixedIdeo: jobRitual.Ritual.ideo, forcedXenotype: chosenXenotype));
                QuestUtility.GenerateQuestAndMakeAvailable(QuestScriptDefOf.WandererJoins, vars);
                extraOutcomeDesc = __instance.def.letterInfoText;
            }
            else
                extraOutcomeDesc = (string) null;

            return false;
        }
    }
}
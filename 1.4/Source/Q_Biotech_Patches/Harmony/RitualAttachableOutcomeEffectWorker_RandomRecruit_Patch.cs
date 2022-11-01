using System;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
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
                var vars = new Slate();
                vars.Set<Map>("map", jobRitual.Map);

                var randPawn = (from pawn in totalPresence.Keys.ToList() select pawn).RandomElement();
                if (randPawn.genes.UniqueXenotype)
                { // xenotype is custom
                    var filePath = GenFilePaths.AbsFilePathForXenotype(randPawn.genes.xenotypeName);
                    CustomXenotype chosenXenotype = null;
                    if (GameDataSaveLoader.TryLoadXenotype(filePath, out chosenXenotype)) // try to load xenotype def
                        vars.Set<PawnGenerationRequest>("overridePawnGenParams",
                            new PawnGenerationRequest(PawnKindDefOf.Villager, forceGenerateNewPawn: true,
                                colonistRelationChanceFactor: 20f, fixedIdeo: jobRitual.Ritual.ideo,
                                forcedCustomXenotype: chosenXenotype));
                    else // loading xenotype def failed; fall back to baseliner
                        vars.Set<PawnGenerationRequest>("overridePawnGenParams",
                            new PawnGenerationRequest(PawnKindDefOf.Villager, forceGenerateNewPawn: true,
                                colonistRelationChanceFactor: 20f, fixedIdeo: jobRitual.Ritual.ideo));
                }
                else
                { // xenotype is non-baseliner but pre-defined
                    vars.Set<PawnGenerationRequest>("overridePawnGenParams",
                        new PawnGenerationRequest(PawnKindDefOf.Villager, forceGenerateNewPawn: true,
                            colonistRelationChanceFactor: 20f, fixedIdeo: jobRitual.Ritual.ideo,
                            forcedXenotype: randPawn.genes.Xenotype));
                }

                QuestUtility.GenerateQuestAndMakeAvailable(QuestScriptDefOf.WandererJoins, vars);
                extraOutcomeDesc = __instance.def.letterInfoText;
            }
            else
            {
                extraOutcomeDesc = (string)null;
            }

            return false;
        }
    }
}
using System.Reflection;
using HarmonyLib;
using Verse;

namespace Q_Biotech_Patches
{
    [StaticConstructorOnStartup]
    public class Main
    {
        static Main()
        {
            new Harmony("qbiotechpatches.mod").PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
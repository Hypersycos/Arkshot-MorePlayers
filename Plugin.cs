using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace MorePlayers
{
    [BepInPlugin("hypersycos.plugins.arkshot.moreplayers", "More Players", "1.0.0")]
    [BepInProcess("Arkshot.exe")]
    public class Plugin : BaseUnityPlugin
    {
        private static ConfigEntry<int> configMaxPlayerCount;
        private void Awake()
        {
            configMaxPlayerCount = Config.Bind("General",
                "MaxPlayerCount",
                4,
                "The new max player count to use");
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Harmony.CreateAndPatchAll(typeof(Plugin));
        }

        [HarmonyPatch(typeof(mainMenu), "createRoom")]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach(CodeInstruction instruction in instructions)
            { 
                if (instruction.opcode == OpCodes.Ldc_I4_4)
                {
                    yield return new CodeInstruction(OpCodes.Ldc_I4, configMaxPlayerCount.Value);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}

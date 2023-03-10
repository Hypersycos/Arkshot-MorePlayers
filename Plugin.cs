using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MorePlayers
{
    [BepInPlugin("hypersycos.plugins.arkshot.moreplayers", "More Players", "1.1.0")]
    [BepInDependency("hypersycos.plugins.arkshot.multiplayersync")]
    [BepInProcess("Arkshot.exe")]
    public class Plugin : BaseUnityPlugin
    {
        private static ConfigEntry<int> configMaxPlayerCount;
        public static Plugin Instance;
        private void Awake()
        {
            configMaxPlayerCount = Config.Bind("General",
                "MaxPlayerCount",
                4,
                "The new max player count to use");

            MultiplayerSync.MultiplayerSync.RegisterConditionalRequirement(this, () => configMaxPlayerCount.Value > 4);

            // Plugin startup logic
            Instance = this;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Harmony.CreateAndPatchAll(typeof(Patches));
        }

        [HarmonyPatch]
        public class Patches
        {
            [HarmonyTranspiler]
            [HarmonyPatch(typeof(mainMenu), "createRoom")]
            static IEnumerable<CodeInstruction> createRoom_Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                foreach (CodeInstruction instruction in instructions)
                {
                    if (instruction.opcode == OpCodes.Ldc_I4_4)
                    { //overwrites the room options max player count
                        yield return new CodeInstruction(OpCodes.Ldc_I4, configMaxPlayerCount.Value);
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }

            [HarmonyTranspiler]
            [HarmonyPatch(typeof(menuScript), "spawnPlayer", MethodType.Enumerator)]
            static IEnumerable<CodeInstruction> spawnPlayer_Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                bool isFirst = true;
                CodeInstruction loadThis = null;
                CodeInstruction loadSpawnParent = null;
                CodeInstruction getChildCount = null;
                foreach (CodeInstruction instruction in instructions)
                {
                    if (getChildCount == null && instruction.opcode == OpCodes.Callvirt && instruction.operand.ToString() == "Int32 get_childCount()")
                    { //grab getChildCount IL
                        getChildCount = instruction;
                    }
                    if (instruction.opcode == OpCodes.Ldfld)
                    {
                        if (instruction.operand.ToString() == "System.Int32 myID")
                        {
                            yield return instruction;
                            if (isFirst)
                            {   //Makes sure respawn delay is id % 4, rather than just id
                                //This prevents an infinite loop of respawning if a player with id > 4 dies
                                isFirst = false;
                                yield return new CodeInstruction(OpCodes.Ldc_I4_4);
                                yield return new CodeInstruction(OpCodes.Rem);
                            }
                            else
                            {   //At the start of the round, and if unable to find a non-overlapping spawn, player id is used as an index
                                //Many maps only have 4 spawn points, but some have more
                                //This replaces the id with id % spawnParent.childCount, preventing errors but also using all available spawns
                                yield return new CodeInstruction(OpCodes.Ldarg_0);
                                yield return loadThis;
                                yield return loadSpawnParent;
                                yield return getChildCount;
                                yield return new CodeInstruction(OpCodes.Rem);
                            }
                        }
                        else
                        {
                            if (loadThis == null && instruction.operand.ToString() == "menuScript <>f__this")
                            { //grab IL for keyword this
                                loadThis = instruction;
                            }
                            else if (loadSpawnParent == null && instruction.operand.ToString() == "UnityEngine.Transform spawnParent")
                            { //grab IL for spawnParent
                                loadSpawnParent = instruction;
                            }
                            yield return instruction;
                        }
                    }
                    else
                    {
                        yield return instruction;
                    }
                }
            }
        }
    }
}

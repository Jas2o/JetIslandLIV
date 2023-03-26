using HarmonyLib;
using MelonLoader;

namespace JetIslandLIV {
    [HarmonyPatch]
    public static class Patches {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StartGameScript), "PlayOffline")]
        private static void SetUpLiv(StartGameScript __instance) {
            MelonLogger.Msg("## Patches : StartGameScript-PlayOffline-SetUpLiv ##");
            JetIslandLIVMod.OnPlayerReady();
        }

    }
}

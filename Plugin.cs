using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using static ToolItem;

namespace NoShardTools;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Hollow Knight Silksong.exe")]
public class Plugin : BaseUnityPlugin
{

    private void Awake()
    {
        Harmony.CreateAndPatchAll(typeof(Plugin));
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    [HarmonyPatch(typeof(PlayerData), "TakeShards")]
    [HarmonyPrefix]
    private static void TakeShardsPrefix(PlayerData __instance, ref int amount)
    {
        amount = 0;
    }

    // [HarmonyPatch(typeof(ToolItem), "ReloadSingle")]
    // [HarmonyPrefix]
    // private static void ReloadSinglePrefix(ToolItem __instance, ref int amount)
    // {                
    //     ToolItemsData.Data savedData = __instance.SavedData;
    //     savedData.AmountLeft++;
    //     int toolStorageAmount = ToolItemManager.GetToolStorageAmount(__instance);
    //     if (savedData.AmountLeft > toolStorageAmount)
    //     {
    //         savedData.AmountLeft = toolStorageAmount;
    //     }

    //     __instance.SavedData = savedData;
    //     if (__instance.ReplenishResource == ReplenishResources.Money)
    //     {
    //         CurrencyManager.TakeCurrency(1, (CurrencyType)__instance.ReplenishResource);
    //     }
        
    // }

}

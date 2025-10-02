using System;
using System.Collections.Generic;
using System.Linq;
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

    [HarmonyPatch(typeof(ToolItemManager), "TryReplenishTools")]
    [HarmonyPrefix]
    private static bool TryReplenishToolsPrefix(bool __result)
    {

        bool itemsHaveBeenReplenished = false;

        if (string.IsNullOrEmpty(PlayerData.instance.CurrentCrestID))
        {            
            return true;
        }

        List<ToolItem> currentEquippedTools = GetCurrentEquippedTools();
        if (currentEquippedTools == null)
        {            
            return true;
        }

        currentEquippedTools.RemoveAll((ToolItem tool) => tool == null || !tool.IsAutoReplenished());

        foreach (ToolItem item in currentEquippedTools)
        {

            ToolItemsData.Data toolData = PlayerData.instance.GetToolData(item.name);
            int toolStorageAmount = ToolItemManager.GetToolStorageAmount(item);

            if (toolStorageAmount > toolData.AmountLeft)
            {
                toolData.AmountLeft = toolStorageAmount;
                PlayerData.instance.Tools.SetData(item.name, toolData);
                itemsHaveBeenReplenished = true;

                ToolItemManager.ReportAllBoundAttackToolsUpdated();
    	        ToolItemManager.SendEquippedChangedEvent(force: true);
            }            

        }

        if (itemsHaveBeenReplenished)
        {
            __result = true;
        }

        return true;

    }

    private static List<ToolItem> GetCurrentEquippedTools()
    {
        List<ToolItem> obj = ToolItemManager.GetEquippedToolsForCrest(PlayerData.instance.CurrentCrestID) ?? new List<ToolItem>();

        IEnumerable<ToolItem> collection = from data in PlayerData.instance.ExtraToolEquips.GetValidDatas((ToolCrestsData.SlotData data) => !string.IsNullOrEmpty(data.EquippedTool))
                                           select ToolItemManager.GetToolByName(data.EquippedTool);
        obj.AddRange(collection);

        return obj;
    }

}

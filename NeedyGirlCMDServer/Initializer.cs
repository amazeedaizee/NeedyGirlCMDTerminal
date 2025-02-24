using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ngov3;
using UnityEngine;

namespace NeedyGirlCMDServer
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    [BepInProcess("Windose.exe")]
    public class Initializer : BaseUnityPlugin
    {
        public const string pluginGuid = "needy.girl.commandserver";
        public const string pluginName = "Command Server";
        public const string pluginVersion = "1.0.0.0";

        public static ManualLogSource logger;
        public static PluginInfo PInfo { get; private set; }
        public void Awake()
        {
            PInfo = Info;
            logger = Logger;

            this.gameObject.hideFlags = HideFlags.HideAndDontSave;
            Logger.LogInfo("Wow, now you can control the game through stuff like command prompt! Interesting...");
            Harmony harmony = new Harmony(pluginGuid);
            harmony.PatchAll();
        }

        public void Start()
        {
            MsgManager.InitializeMsgs();
            MyPicturesCommands.resourceList = ImageViewerHelper.LoadResourcesList();
            logger.LogInfo("Starting...");
            ConnectionManager.StartServer();
        }

        public void OnApplicationQuit()
        {
            if (ConnectionManager.client != null && ConnectionManager.client.Connected)
            {

                ConnectionManager.cts.Cancel();
                ConnectionManager.client.Client.Close();
                ConnectionManager.client.Client.Dispose();
                ConnectionManager.pipe.Stop();

            }
        }

    }

}



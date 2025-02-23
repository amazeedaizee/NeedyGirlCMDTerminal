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
        public const string pluginGuid = "needy.girl.commandprompt";
        public const string pluginName = "Command Prompt";
        public const string pluginVersion = "1.0.0.0";

        public static ManualLogSource logger;
        public static PluginInfo PInfo { get; private set; }
        public void Awake()
        {
            PInfo = Info;
            logger = Logger;

            this.gameObject.hideFlags = HideFlags.HideAndDontSave;
            Logger.LogInfo("Wow, now you can control the game through the system console! Interesting...");
            Harmony harmony = new Harmony(pluginGuid);
            harmony.PatchAll();
        }

        public void Start()
        {
            MyPicturesCommands.resourceList = ImageViewerHelper.LoadResourcesList();
            logger.LogInfo("Starting...");
            ConnectionManager.StartServer();
        }

        void LoadResources()
        {

        }

        public async void OnApplicationQuit()
        {
            if (ConnectionManager.client != null && ConnectionManager.client.Connected)
            {
                ConnectionManager.client.Close();
                ConnectionManager.client.Dispose();
                ConnectionManager.tcpListener.Stop();
            }
        }
    }

}



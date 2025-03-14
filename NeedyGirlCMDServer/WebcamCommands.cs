﻿using Cysharp.Threading.Tasks;
using ngov3;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

namespace NeedyGirlCMDServer
{
    internal class WebcamCommands
    {
        internal static async UniTask<string> ControlWebcam(string input)
        {
            var seperator = new Regex(@"\s+");
            string[] commands = seperator.Split(input, 4);
            bool isDataActive = SceneManager.GetActiveScene().name != "BiosToLoad" && SceneManager.GetActiveScene().name != "ChoozeZip";
            if (!isDataActive)
            {
                return MsgManager.SendMessage(ServerMessage.CMD_SPECIFIC_BUSY);
            }
            if (!SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.Webcam))
            {
                return MsgManager.SendMessage(ServerMessage.WEBCAM_NOT_ACTIVE);
            }
            var ame = SingletonMonoBehaviour<WindowManager>.Instance.WindowList.Find(t => t.appType == AppType.Webcam);
            if (commands.Length == 1)
            {
                ame.Touched();
                return "";
            }

            WindowCommands.ChangeWindowState(ame, commands[1]);
            try
            {
                if (commands[1] == "pat" || commands[1] == "p")
                {
                    Amehead head = ame.nakamiApp.GetComponent<App_Webcam>().AmeHead.GetComponent<Amehead>();
                    ame.Touched();
                    if (commands.Length > 2 && int.TryParse(commands[2], out int num))
                    {
                        if (num < 1)
                        {
                            return MsgManager.SendMessage(ServerMessage.WEBCAM_PAT_OUTRANGE);
                        }
                        for (int i = 0; i < num; i++)
                        {
                            await UniTask.Delay(200);
                            head._button.onClick.Invoke();
                            head._button.onClick.Invoke();
                        }
                        return MsgManager.SendMessage(ServerMessage.WEBCAM_PAT_COUNT, num);
                    }
                    else head._button.onClick.Invoke();
                }
                else if (commands[1] == "random")
                {
                    ame.Touched();
                    SingletonMonoBehaviour<WebCamManager>.Instance.RandomizeAmeAnimation();
                }
            }
            catch { }
            return "";
        }
    }
}

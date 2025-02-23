using Cysharp.Threading.Tasks;
using NGO;
using ngov3;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NeedyGirlCMDServer
{
    internal class JineCommands
    {
        readonly static string[] jineSticker = { "sticker", "s" };
        readonly static string[] jineMessage = { "message", "msg", "m" };
        readonly static string[] jineChoose = { "choose", "c" };
        readonly static string[] jineRead = { "read", "r" };
        internal static async UniTask<string> SelectJineCommand(string input)
        {
            IWindow window;
            string customMsg;
            string userToRead;
            var seperator = new Regex(@"\s+");
            string[] commands = seperator.Split(input, 3);
            bool isDataActive = SceneManager.GetActiveScene().name != "BiosToLoad" && SceneManager.GetActiveScene().name != "ChoozeZip";
            EndingType currentEnding = SingletonMonoBehaviour<EventManager>.Instance.nowEnding;
            bool isHorror = SingletonMonoBehaviour<EventManager>.Instance.isHorror;
            bool isJineRequiredForEnd = currentEnding == EndingType.Ending_Work || currentEnding == EndingType.Ending_Needy || currentEnding == EndingType.Ending_Normal || currentEnding == EndingType.Ending_Yarisute;
            bool isWindowActive = SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.Jine);
            if (!isDataActive || (!SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable && !isWindowActive && !isJineRequiredForEnd && !isHorror))
            {
                return ErrorMessages.CMD_SPECIFIC_BUSY;
            }
            if (commands.Length == 1)
            {
                if (!isWindowActive)
                {
                    if (!SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable)
                        return ErrorMessages.CMD_SPECIFIC_BUSY;
                    SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.Jine);
                }
                else SingletonMonoBehaviour<WindowManager>.Instance.GetWindowFromApp(AppType.Jine).Touched();
                return "";
            }
            if (commands.Length == 2 && isWindowActive)
            {
                if (isJineRequiredForEnd || isHorror)
                    return "Can't modify the Jine window now.";
                window = SingletonMonoBehaviour<WindowManager>.Instance.GetWindowFromApp(AppType.Jine);
                if (WindowCommands.IsWindowScroll(window, commands[1]))
                {
                    return "";
                }
                if (!WindowCommands.ChangeWindowState(window, commands[1]))
                {
                    return "Invalid command for the Jine window.";
                }
                return "";
            }
            if (commands.Length < 3)
            {
                return "This command (when not just opening the Jine window or scrolling) requires at least 3 arguments.";
            }
            if (CommandManager.IsInputMatchCmd(commands[1], jineSticker))
            {
                var sticker = seperator.Split(commands[2], 2);
                return await SendSticker(sticker[0]);
            }

            if (CommandManager.IsInputMatchCmd(commands[1], jineMessage))
            {
                customMsg = commands[2];
                customMsg.TrimStart(' ', '\n', '\r');
                return await SendMessage(customMsg);
            }

            if (CommandManager.IsInputMatchCmd(commands[1], jineChoose))
            {
                var choice = seperator.Split(commands[2], 2);
                return await ChooseOption(choice[0]);
            }

            if (CommandManager.IsInputMatchCmd(commands[1], jineRead))
            {
                userToRead = commands[2];
                return ReadMessage(userToRead);
            }
            if (commands[1] == "history")
            {
                var count = seperator.Split(commands[2], 2);
                if (count[0] == "count")
                    return HistoryCount();
            }
            return ErrorMessages.INVALID_CMD;
        }

        internal static string HistoryCount()
        {
            var jineHistory = SingletonMonoBehaviour<JineManager>.Instance.history.FindAll(j => j.user == JineUserType.ame || j.user == JineUserType.pi);
            return $"Number of total Jine Messages: {jineHistory.Count}";
        }
        internal static async UniTask<string> SendSticker(string input)
        {
            WindowManager windowManager;
            JineManager jineManager;
            IWindow window;
            JineStampView2D jineStampView2D;
            windowManager = SingletonMonoBehaviour<WindowManager>.Instance;
            if (!int.TryParse(input, out var num))
                return "Sticker must be a number.";
            num--;
            if (!SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.Jine))
            {
                if (!SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable)
                    return "Can't open Jine from the command line now.";
                windowManager.NewWindow(AppType.Jine);
                await UniTask.Delay(50);
            }
            window = windowManager.GetWindowFromApp(AppType.Jine);
            jineStampView2D = window.nakamiApp.GetComponentInChildren<JineStampView2D>();
            jineManager = SingletonMonoBehaviour<JineManager>.Instance;
            if (jineManager.waitStatus.Value != JineManager.WaitStatusType.Stamp)
            {
                return "Jine view is not accepting stickers.";
            }
            if (num > 8 || num < 0)
            {
                return "Sticker is out of range.";
            }
            await UniTask.Delay(300);
            jineStampView2D.sendStamp((StampType)num);
            return "";
        }

        internal static async UniTask<string> SendMessage(string input)
        {
            WindowManager windowManager;
            JineManager jineManager;
            IWindow window;
            JineView2D jineView2D;
            windowManager = SingletonMonoBehaviour<WindowManager>.Instance;
            if (!SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.Jine))
            {
                if (!SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable)
                    return "Can't open Jine from the command line now.";
                windowManager.NewWindow(AppType.Jine);
                await UniTask.Delay(50);
            }
            window = windowManager.GetWindowFromApp(AppType.Jine);
            jineView2D = window.nakamiApp.GetComponentInChildren<JineView2D>();
            jineManager = SingletonMonoBehaviour<JineManager>.Instance;
            if (jineManager.waitStatus.Value != JineManager.WaitStatusType.FreeForm || !jineView2D._piFreeform.gameObject.activeInHierarchy)
            {
                return "Jine view is not accepting custom messages.";
            }
            await UniTask.Delay(300);
            jineView2D._inputField.text = input;
            jineView2D.sendMessage(input);
            return "";
        }

        internal static async UniTask<string> ChooseOption(string input)
        {
            WindowManager windowManager;
            JineManager jineManager;
            IWindow window;
            JineView2D jineView2D;
            if (!int.TryParse(input, out var num))
                return "Option must be a number.";
            num--;
            windowManager = SingletonMonoBehaviour<WindowManager>.Instance;
            if (!SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.Jine))
            {
                if (!SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable)
                    return "Can't open Jine from the command line now.";
                windowManager.NewWindow(AppType.Jine);
                await UniTask.Delay(50);
            }
            window = windowManager.GetWindowFromApp(AppType.Jine);
            jineView2D = window.nakamiApp.GetComponentInChildren<JineView2D>();
            jineManager = SingletonMonoBehaviour<JineManager>.Instance;
            if (jineView2D._selectableObjects.Count == 0 || jineManager.waitStatus.Value != JineManager.WaitStatusType.Option)
            {
                return "Jine view is not showing any options.";
            }
            if (num >= jineView2D._selectableObjects.Count || num < 0)
                return "Option doesn't exist.";
            await UniTask.Delay(300);
            jineView2D._selectableObjects[num].GetComponentInChildren<Button>().onClick.Invoke();
            return "";

        }

        internal static string ReadMessage(string input)
        {

            WindowManager windowManager;
            windowManager = SingletonMonoBehaviour<WindowManager>.Instance;
            if (!SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.Jine))
            {
                if (!SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable)
                    return "Can't open Jine from the command line now.";
                windowManager.NewWindow(AppType.Jine);
            }
            if (input == "last")
            {
                return ReadMessage(true);
            }
            else if (input == "first")
            {
                return ReadMessage(false);
            }
            else if (input == "first ame")
            {
                return ReadMessage(false, JineUserType.ame);
            }
            else if (input == "last ame")
            {
                return ReadMessage(true, JineUserType.ame);
            }
            else if (input == "first p" || input == "first p-chan")
            {
                return ReadMessage(false, JineUserType.pi);
            }
            else if (input == "last p" || input == "last p-chan")
            {
                return ReadMessage(true, JineUserType.pi);
            }
            else
            {
                if (!int.TryParse(input, out var num))
                    return "Couldn't find a valid message!.";
                return ReadMessage(num);
            }
        }
        internal static string ReadMessage(int input)
        {
            string user;
            string message = "";
            JineManager jineManager;
            JineData jine;
            List<JineData> jineHistory;
            input--;
            jineManager = SingletonMonoBehaviour<JineManager>.Instance;
            jineHistory = jineManager.history.FindAll(j => j.user == JineUserType.ame || j.user == JineUserType.pi);
            if (jineHistory.Count == 0)
            {
                return "Jine History is empty.";
            }
            if (input >= jineHistory.Count || input < 0)
                return "Jine index value is out of bounds.";
            jine = jineHistory[input];
            user = jine.user == JineUserType.ame ? "Ame:" : "You:";
            if (jine.responseType == ResponseType.Stamp)
            {
                message = jine.stampType.ToString();
            }
            else if (jine.responseType == ResponseType.Freeform)
            {
                message = jine.freeMessage;
            }
            else if (jine.responseType == ResponseType.IdMessage)
            {
                message = JineDataConverter.GetJineTextFromTypeId(jine.id);
            }
            return $"{user}\n{message}";
        }

        internal static string ReadMessage(bool isLastMsg, JineUserType userType = JineUserType.separator)
        {
            string user = "";
            string message = "";
            JineManager jineManager;
            List<JineData> jineHistory;
            jineManager = SingletonMonoBehaviour<JineManager>.Instance;
            jineHistory = jineManager.history.FindAll(j => (j.user == JineUserType.ame || j.user == JineUserType.pi) && j.responseType != ResponseType.Stamp);
            if (jineHistory.Count == 0)
            {
                return "Jine History is empty.";
            }
            if (isLastMsg)
            {
                jineHistory.Reverse();
            }
            for (int i = 0; i < jineHistory.Count; i++)
            {
                if (userType == JineUserType.ame && jineHistory[i].user != JineUserType.ame)
                    continue;
                if (userType == JineUserType.pi && jineHistory[i].user != JineUserType.pi)
                    continue;
                user = jineHistory[i].user == JineUserType.ame ? "Ame:" : "You:";
                if (jineHistory[i].responseType == ResponseType.Freeform)
                {
                    message = jineHistory[i].freeMessage;
                    break;
                }
                if (jineHistory[i].id == NGO.JineType.None)
                    continue;
                message = JineDataConverter.GetJineTextFromTypeId(jineHistory[i].id);
                break;
            }
            return $"{user}\n{message}";
        }
    }
}

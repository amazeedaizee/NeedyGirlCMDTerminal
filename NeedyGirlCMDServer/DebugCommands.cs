using Cysharp.Threading.Tasks;
using ngov3;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NeedyGirlCMDServer
{
    internal class DebugCommands
    {
        const float MAX_GAME_SPEED = 20f;
        const float MIN_GAME_SPEED = 0.1f;

        readonly static string[] playCommand = { "play" };
        readonly static string[] speedCommand = { "speed" };
        readonly static string[] setCommand = { "set" };

        readonly static string[] setFollowers = { "followers", "f" };
        readonly static string[] setStress = { "stress", "s" };
        readonly static string[] setAffection = { "affection", "love", "a", "l" };
        readonly static string[] setDarkness = { "darkness", "dark", "d" };
        readonly static string[] setDay = { "day" };
        readonly static string[] setSexCount = { "sex" };
        readonly static string[] setTestLevel = { "streamlevel" };
        internal static async UniTask<string> StartDebugCommand(string input)
        {
            string activeScene = SceneManager.GetActiveScene().name;
            var seperator = new Regex(@"\s+");
            string[] commands = seperator.Split(input, 5);
            if (activeScene != "Window2DTestScene" && !DebugMode.IsDebugMode)
            {
                return MsgManager.SendMessage(ServerMessage.DEBUG_NOT_ACTIVE);
            }
            if (commands.Length == 1)
            {
                if (activeScene == "BiosToLoad")
                {
                    SingletonMonoBehaviour<Boot>.Instance.OnDebugButtonClicked();
                }
                else
                {
                    SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.TaskManagerForDebug);
                }
                return "";
            }
            if (commands.Length < 3)
            {
                return MsgManager.SendMessage(ServerMessage.CMD_WRONG_ARGS);
            }
            if (activeScene == "Window2DTestScene")
            {
                if (CommandManager.IsInputMatchCmd(commands[1], playCommand))
                {
                    return await PlayEvent(commands[2]);
                }
                else if (CommandManager.IsInputMatchCmd(commands[1], speedCommand))
                {
                    return ChangeGameSpeed(commands[2]);
                }
                else if (CommandManager.IsInputMatchCmd(commands[1], CommandManager.webcamCommand))
                {
                    return PlayAnimation(commands[2]);
                }
            }
            if (activeScene.Contains("Window") && CommandManager.IsInputMatchCmd(commands[1], setCommand))
            {
                if (commands.Length < 4)
                {
                    return MsgManager.SendMessage(ServerMessage.CMD_WRONG_ARGS);
                }
                else return ChangeStatusToNumber(commands);
            }
            return MsgManager.SendMessage(ServerMessage.INVALID_CMD);
        }

        static string PlayAnimation(string anim)
        {
            IWindow cam = null;
            if (!SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.Webcam))
            {
                cam = SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.Webcam);
            }
            else { cam = SingletonMonoBehaviour<WindowManager>.Instance.GetWindowFromApp(AppType.Webcam); }
            try
            {
                cam.Touched();
                SingletonMonoBehaviour<WebCamManager>.Instance.PlayAnim(anim);
            }
            catch (NullReferenceException) { return MsgManager.SendMessage(ServerMessage.DEBUG_CAM_NOT_ACTIVE); }
            catch { return MsgManager.SendMessage(ServerMessage.WEBCAM_ANIM_ERROR); }
            return "";
        }

        static async UniTask<string> PlayEvent(string ngoEvent)
        {
            int chosenIndex;
            Dropdown dropdown;
            var events = GameObject.Find("events").GetComponent<EventChooser>();
            if (!events.events.Contains(ngoEvent))
            {
                return MsgManager.SendMessage(ServerMessage.DEBUG_EVENT_NOT_FOUND);
            }
            dropdown = events.GetComponent<Dropdown>();
            chosenIndex = dropdown.options.FindIndex(x => x.text == ngoEvent);
            dropdown.value = chosenIndex;
            EventStart(events).Forget();
            // Initializer.logger.LogInfo("Event start");
            return "";
        }
        static async UniTask EventStart(EventChooser events)
        {
            events.OnSelected();
            await UniTask.Yield();
        }
        static string ChangeGameSpeed(string input)
        {
            if (!float.TryParse(input, out float speed))
            {
                return MsgManager.SendMessage(ServerMessage.DEBUG_SPEED_ARG_ERROR);
            }
            string message = "";
            if (speed > MAX_GAME_SPEED)
            {
                message = MsgManager.SendMessage(ServerMessage.DEBUG_SPEED_HIGH, MAX_GAME_SPEED);
                speed = MAX_GAME_SPEED;
            }
            if (speed < MIN_GAME_SPEED)
            {
                message = MsgManager.SendMessage(ServerMessage.DEBUG_SPEED_LOW, MIN_GAME_SPEED);
                speed = MIN_GAME_SPEED;
            }
            Time.timeScale = speed;
            return message;
        }

        static string ChangeStatusToNumber(string[] commands)
        {
            int stat;
            var statusManager = SingletonMonoBehaviour<StatusManager>.Instance;
            if (!int.TryParse(commands[3], out stat))
            {
                return MsgManager.SendMessage(ServerMessage.DEBUG_STATUS_NAN);
            }
            if (CommandManager.IsInputMatchCmd(commands[2], setFollowers))
            {
                return ChangeStatusToNumber(StatusType.Follower, stat);
            }
            else if (CommandManager.IsInputMatchCmd(commands[2], setStress))
            {
                return ChangeStatusToNumber(StatusType.Stress, stat);
            }
            else if (CommandManager.IsInputMatchCmd(commands[2], setAffection))
            {
                return ChangeStatusToNumber(StatusType.Love, stat);
            }
            else if (CommandManager.IsInputMatchCmd(commands[2], setDarkness))
            {
                return ChangeStatusToNumber(StatusType.Yami, stat);
            }
            else if (CommandManager.IsInputMatchCmd(commands[2], setDay))
            {
                return ChangeStatusToNumber(StatusType.DayIndex, stat);
            }
            else if (CommandManager.IsInputMatchCmd(commands[2], setSexCount))
            {
                return ChangeStatusToNumber(StatusType.MadeLoveCounter, stat);
            }
            else if (CommandManager.IsInputMatchCmd(commands[2], setTestLevel))
            {
                return ChangeStatusToNumber(StatusType.testAlphaLevel, stat);
            }
            return MsgManager.SendMessage(ServerMessage.INVALID_CMD);

            string ChangeStatusToNumber(StatusType status, int stat)
            {
                int maxFollowers = statusManager.GetMaxStatus(status);
                if (stat < 0)
                {
                    stat = 0;
                }
                if (stat > maxFollowers)
                {
                    stat = maxFollowers;
                }
                statusManager.UpdateStatusToNumber(status, stat);
                return "";
            }
        }
    }
}

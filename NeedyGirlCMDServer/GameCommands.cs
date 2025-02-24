using Cysharp.Threading.Tasks;
using HarmonyLib;
using ngov3;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace NeedyGirlCMDServer
{
    [HarmonyPatch]
    internal class MouseClick
    {
        internal static bool isClicked;

        internal static async UniTask FakeMouseClick()
        {
            var boot = SingletonMonoBehaviour<Boot>.Instance;
            var zips = SingletonMonoBehaviour<EndingOmake>.Instance;
            var endingScreen = SingletonMonoBehaviour<EndingManager>.Instance;
            if (boot != null && (IsBiosClickable(boot) || IsSplashClickable(boot)))
                return;
            if (zips != null && IsUnzippedClickable(zips))
                return;
            if (endingScreen != null && IsBlueScreenActive(endingScreen))
                return;
            if (IsCoverClickable())
                return;
            isClicked = true;
            //Initializer.logger.LogInfo($"Click!");
            await UniTask.Delay(1);
            isClicked = false;
        }

        internal static bool IsBiosClickable(Boot boot)
        {
            try
            {
                if (!boot.Bios.interactable)
                    return false;
                boot.Bios.gameObject.GetComponent<Button>().onClick.Invoke();
                return true;
            }
            catch { return false; }
        }

        internal static bool IsBlueScreenActive(EndingManager endingScreen)
        {
            try
            {
                endingScreen.gameObject.GetComponent<Button>().onClick.Invoke();
                return true;
            }
            catch { return false; }
        }

        internal static bool IsUnzippedClickable(EndingOmake zips)
        {
            try
            {
                if (!zips._continue.interactable)
                    return false;
                zips._continue.gameObject.GetComponent<Button>().onClick.Invoke();
                return true;
            }
            catch { return false; }
        }
        internal static bool IsSplashClickable(Boot boot)
        {
            try
            {
                if (!boot.Splash.interactable)
                    return false;
                boot.Splash.gameObject.GetComponent<Button>().onClick.Invoke();
                return true;
            }
            catch { return false; }

        }
        internal static bool IsCoverClickable()
        {
            bool isDataActive = SceneManager.GetActiveScene().name != "BiosToLoad" && SceneManager.GetActiveScene().name != "ChoozeZip";
            if (!isDataActive) return false;
            var cover = SingletonMonoBehaviour<EventManager>.Instance.cover;
            if (cover.interactable)
            {
                ExecuteEvents.Execute(cover.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
                ExecuteEvents.Execute(cover.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
                ExecuteEvents.Execute(cover.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
                ExecuteEvents.Execute(cover.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
                return true;
            }
            return false;
        }

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(Player), nameof(Player.GetButtonDown), new Type[] { typeof(string) })]
        internal static void SniffFakeClick_R(ref bool __result, string actionName)
        {
            if (actionName == "Click" && isClicked)
            {
                __result = true;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Input), nameof(Input.GetMouseButtonDown), new Type[] { typeof(int) })]
        internal static void SniffFakeClick_U(ref bool __result, int button)
        {
            if (button == 0 && isClicked)
            {
                __result = true;
            }
        }

    }
    internal class GameCommands
    {

        internal static string RestartGame(string input)
        {
            WindowManager windowManager = SingletonMonoBehaviour<WindowManager>.Instance;
            char[] seperator = { ' ' };
            string[] commands = input.Split(seperator, 3);
            RebootDialog rebootDialog = new RebootDialog();
            bool isDataActive = SceneManager.GetActiveScene().name != "BiosToLoad" && SceneManager.GetActiveScene().name != "ChoozeZip";
            if (isDataActive &&
                !SingletonMonoBehaviour<EventManager>.Instance.isTestScene &&
                !SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable)
            {
                return MsgManager.SendMessage(ServerMessage.RESTART_BUSY);
            }
            if (commands.Length == 2 && (commands[1] == "f" || commands[1] == "force"))
            {
                if (!isDataActive) SceneManager.LoadScene("BiosToLoad");
                else rebootDialog.OnSubmit();
                return MsgManager.SendMessage(ServerMessage.RESTART_CONFIRMED);
            }
            else if (isDataActive &&
                (SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable ||
                SingletonMonoBehaviour<EventManager>.Instance.isTestScene))
            {
                if (!windowManager.isAppOpen(AppType.RebootDialog))
                    windowManager.NewWindow(AppType.RebootDialog);
                else
                {
                    var window = windowManager.GetWindowFromApp(AppType.RebootDialog);
                    window.Touched();
                }
                return "";
            }
            return MsgManager.SendMessage(ServerMessage.RESTART_DIALOG_BUSY);
        }

        internal static string ShutDownGame(string input)
        {
            WindowManager windowManager = SingletonMonoBehaviour<WindowManager>.Instance;
            char[] seperator = { ' ' };
            string[] commands = input.Split(seperator, 3);
            bool isDataActive = SceneManager.GetActiveScene().name != "BiosToLoad" && SceneManager.GetActiveScene().name != "ChoozeZip";
            if (commands.Length == 2 && (commands[1] == "f" || commands[1] == "force"))
            {
                Application.Quit();
                return "";
            }
            else if (SceneManager.GetActiveScene().name == "BiosToLoad" || isDataActive && SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable)
            {
                if (!windowManager.isAppOpen(AppType.RebootDialog))
                    windowManager.NewWindow(AppType.ShutDownDialog);
                else
                {
                    var window = windowManager.GetWindowFromApp(AppType.ShutDownDialog);
                    window.Touched();
                }
                return "";
            }
            return MsgManager.SendMessage(ServerMessage.RESTART_SDOWN_DIALOG_BUSY);
        }
    }
}

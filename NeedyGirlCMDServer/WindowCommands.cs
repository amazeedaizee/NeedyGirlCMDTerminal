using NGO;
using ngov3;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace NeedyGirlCMDServer
{
    internal class WindowCommands
    {
        readonly static string[] windowOk = { "ok" };
        readonly static string[] windowCancel = { "cancel" };
        readonly static string[] windowActive = { "active", "a" };
        readonly static string[] windowPrev = { "previous", "prev", "p" };
        readonly static string[] windowNext = { "next", "n" };
        readonly static string[] windowMin = { "minimize", "min", "-" };
        readonly static string[] windowMax = { "maximize", "max", "+" };
        readonly static string[] windowClose = { "close", "x" };

        readonly static string[] scrollUp = { "scrollup" };
        readonly static string[] scrollDown = { "scrolldown" };
        readonly static string[] scrollTop = { "scrolltop" };
        readonly static string[] scrollBottom = { "scrollbottom" };

        static ShortcutInputManager shortcutInputManager = new();
        internal static string SelectWindowCommand(string input, IWindow window)
        {
            var seperator = new Regex(@"\s+");
            string[] commands = seperator.Split(input, 4);
            if (IsWindowScroll(window, commands[0]))
            {
                return "";
            }
            else if (CommandManager.IsInputMatchCmd(commands[0], windowOk))
            {
                return ClickOkButton(window);
            }
            else if (CommandManager.IsInputMatchCmd(commands[0], windowCancel))
            {
                return ClickCancelButton(window);
            }
            if (window.appType == AppType.Jine)
            {
                EndingType currentEnding = SingletonMonoBehaviour<EventManager>.Instance.nowEnding;
                bool isHorror = SingletonMonoBehaviour<EventManager>.Instance.isHorror;
                bool isJineRequiredForEnd = currentEnding == EndingType.Ending_Work || currentEnding == EndingType.Ending_Needy || currentEnding == EndingType.Ending_Normal || currentEnding == EndingType.Ending_Yarisute;
                if (isJineRequiredForEnd || isHorror)
                    return "Can't modify the Jine window now.";
            }
            ChangeWindowState(window, commands[0]);
            return "";
        }
        internal static string SelectWindowCommand(string input)
        {
            IWindow window = null;
            var seperator = new Regex(@"\s+");
            string[] commands = seperator.Split(input, 2);
            if (commands[0] == "toggleall")
            {
                ToggleWindows();
                return "";
            }
            else if (CommandManager.IsInputMatchCmd(commands[0], windowActive))
            {
                window = GetActiveWindow();
            }
            else if (CommandManager.IsInputMatchCmd(commands[0], windowPrev))
            {
                SwitchWindow(false);
                window = GetActiveWindow();
            }
            else if (CommandManager.IsInputMatchCmd(commands[0], windowNext))
            {
                SwitchWindow(true);
                window = GetActiveWindow();
            }
            else if (int.TryParse(commands[0], out int index))
            {
                if (commands.Length > 1)
                {
                    window = GetWindowByIndex(index);
                }
                else
                {
                    SwitchWindow(index - 1);
                    return "";
                }
            }
            if (window != null && commands.Length > 1)
            {
                string com = commands[1];
                return SelectWindowCommand(com, window);
            }
            else if (window != null && commands.Length == 1)
                return "";
            return "Invalid command.";
        }

        internal static bool IsWindowScroll(IWindow window, string command)
        {
            if (CommandManager.IsInputMatchCmd(command, scrollUp))
            {
                MoveScroll(window, true);
            }
            else if (CommandManager.IsInputMatchCmd(command, scrollDown))
            {
                MoveScroll(window, false);
            }
            else if (CommandManager.IsInputMatchCmd(command, scrollTop))
            {
                ScrollToTop(window);
            }
            else if (CommandManager.IsInputMatchCmd(command, scrollBottom))
            {
                ScrollToBottom(window);
            }
            else return false;
            return true;
        }
        internal static void MoveScroll(IWindow window, bool isScrollUp)
        {
            float scrollPosition;
            var scroll = window.nakamiApp.GetComponentInChildren<ScrollRect>();
            if (scroll != null)
            {
                window.Touched();
                scrollPosition = scroll.verticalNormalizedPosition;
                switch (isScrollUp)
                {
                    case true:
                        if ((scrollPosition += 0.1f) == 1)
                            scrollPosition = 1;
                        break;
                    case false:
                        if ((scrollPosition -= 0.1f) == 0)
                            scrollPosition = 0;
                        break;
                }
                scroll.verticalNormalizedPosition = scrollPosition;

            }
        }

        internal static void ScrollToTop(IWindow window)
        {
            var scroll = window.nakamiApp.GetComponentInChildren<ScrollRect>();
            if (scroll != null)
            {
                window.Touched();
                scroll.verticalNormalizedPosition = 1;

            }
        }

        internal static void ScrollToBottom(IWindow window)
        {
            var scroll = window.nakamiApp.GetComponentInChildren<ScrollRect>();
            if (scroll != null)
            {
                window.Touched();
                scroll.verticalNormalizedPosition = 0;

            }
        }

        internal static string ClickOkButton(IWindow window)
        {
            Transform transform;
            Button okButton;
            //Initializer.logger.LogInfo("AppType: " + window.appType.ToString());
            //if (window.appType == AppType.TimePassDialog)
            //{
            //    try
            //    {
            //        window.nakamiApp.transform.Find("Body/ButtonRoot/CloseButton").GetComponent<Button>().onClick.Invoke();
            //        return "";
            //    }
            //    catch { return "This command is invalid for this type of window."; }
            //}
            if (window.appType == AppType.ControlPanel)
            {
                try
                {
                    window.nakamiApp.transform.Find("Body/Stuff/ButtonRoot/ConfirmButton").GetComponent<Button>().onClick.Invoke();
                    return "";
                }
                catch { return "This command is invalid for this type of window."; }
            }
            if (window.appType == AppType.Uratter)
            {
                try
                {
                    var ame = SingletonMonoBehaviour<EndingHappyUraUra>.Instance._uraUraAccount;
                    var follow = SingletonMonoBehaviour<EndingHappyUraUra>.Instance._followRequest;
                    var followers = SingletonMonoBehaviour<EndingHappyUraUra>.Instance._followers;
                    if (followers.activeInHierarchy)
                    {
                        ame.onClick.Invoke();
                    }
                    else
                    {
                        follow.onClick.Invoke();
                    }
                    return "";
                }
                catch { return "This command is invalid for this type of window."; }
            }
            if (window.appType == AppType.Ideon_taiki)
            {
                try
                {
                    window.nakamiApp.transform.Find("Body/Scroll View/start").GetComponent<Button>().onClick.Invoke();
                    return "";
                }
                catch { return "This command is invalid for this type of window."; }
            }
            if (window.appType == AppType.Dinder)
            {
                try
                {
                    window.nakamiApp.transform.Find("BG/ButtonRoot/ActionButton").GetComponent<Button>().onClick.Invoke();
                    return "";
                }
                catch { return "This command is invalid for this type of window."; }
            }
            if (window.appType == AppType.NetaChoose)
            {
                try
                {
                    okButton = window.nakamiApp.GetComponent<NetachipChooser>().StartButton;
                    if (!okButton.isActiveAndEnabled)
                    {
                        return "Can't do this command yet.";
                    }
                    okButton.onClick.Invoke();
                    return "";
                }
                catch { return "This command is invalid for this type of window."; }
            }
            try
            {
                transform = window.nakamiApp.transform.Find("Body/ButtonRoot/ConfirmButton");
                if (transform != null)
                {
                    transform.GetComponent<Button>().onClick.Invoke();
                }
                else
                {
                    transform = window.nakamiApp.transform.GetChild(0).Find("ActionButton");
                    window.nakamiApp.GetComponent<Button>().onClick.Invoke();
                }
                return "";
            }
            catch { return "This command is invalid for this type of window."; }
        }

        internal static string ClickCancelButton(IWindow window)
        {
            Button cancelButton;
            //Initializer.logger.LogInfo("AppType: " + window.appType.ToString());
            //if (window.appType == AppType.TimePassDialog)
            //{
            //    try
            //    {
            //        window.nakamiApp.transform.Find("Body/ButtonRoot/ConfirmButton").GetComponent<Button>().onClick.Invoke();
            //        return "";
            //    }
            //    catch { return "This command is invalid for this type of window."; }
            //}
            if (window.appType == AppType.ControlPanel)
            {
                try
                {
                    window.nakamiApp.transform.Find("Body/Stuff/CloseButton").GetComponent<Button>().onClick.Invoke();
                    return "";
                }
                catch { return "This command is invalid for this type of window."; }
            }
            if (window.appType == AppType.Dinder)
            {
                try
                {
                    window.nakamiApp.transform.Find("BG/ButtonRoot/Nope").GetComponent<Button>().onClick.Invoke();
                    return "";
                }
                catch { return "This command is invalid for this type of window."; }
            }
            if (window.appType == AppType.NetaChoose)
            {
                try
                {
                    cancelButton = window.nakamiApp.GetComponent<NetachipChooser>().backButton;
                    if (!cancelButton.isActiveAndEnabled)
                    {
                        return "Can't do this command yet.";
                    }
                    cancelButton.onClick.Invoke();
                    return "";
                }
                catch { return "This command is invalid for this type of window."; }
            }
            try
            {
                window.nakamiApp.transform.Find("Body/ButtonRoot/CloseButton").GetComponent<Button>().onClick.Invoke();
                return "";
            }
            catch { return "This command is invalid for this type of window."; }
        }

        internal static IWindow GetActiveWindow()
        {
            return SingletonMonoBehaviour<WindowManager>.Instance.WindowList.Find(w => w.activeState == ActiveState.Active);
        }

        internal static IWindow GetWindowByIndex(int input)
        {
            var taskList = SingletonMonoBehaviour<WindowManager>.Instance.TaskBarList;
            try
            {
                input--;
                if (input < 0)
                {
                    input = 0;
                }
                else if (input >= taskList.Count)
                {
                    input = taskList.Count - 1;
                }
                return SingletonMonoBehaviour<WindowManager>.Instance.WindowList[input];
            }
            catch { return null; }
        }

        internal static void ToggleWindows()
        {
            if (SingletonMonoBehaviour<WindowManager>.Instance.WindowList.Where((IWindow w) => w.appType != AppType.Webcam).Any((IWindow w) => w.windowState == WindowState.opened || w.windowState == WindowState.maximized))
            {
                shortcutInputManager.MinimizeAllWindow();
            }
            else
            {
                shortcutInputManager.PopAllWindow();
            }
        }

        internal static void SwitchWindow(int input)
        {
            IWindow window;
            var taskList = SingletonMonoBehaviour<WindowManager>.Instance.TaskBarList;
            if (input < 0)
            {
                input = 0;
            }
            else if (input >= taskList.Count)
            {
                input = taskList.Count - 1;
            }
            window = taskList[input].window;
            if (window.windowState == WindowState.minimized)
            {
                window.Pop();
            }
            window.Touched();

        }

        internal static void SwitchWindow(bool isSwitchNext)
        {
            switch (isSwitchNext)
            {
                case true:
                    shortcutInputManager.WindowChangeToNext();
                    break;
                case false:
                    shortcutInputManager.WindowChangeToPrev();
                    break;
            }
        }

        internal static bool ChangeWindowState(IWindow window, string state)
        {

            try
            {
                if (CommandManager.IsInputMatchCmd(state, windowMin) && window.windowState != WindowState.minimized)
                {
                    if (!window._minimize.interactable)
                        return false;
                    window._minimize.onClick.Invoke();
                    return true;
                }
                if (CommandManager.IsInputMatchCmd(state, windowMax) && window.windowState != WindowState.maximized)
                {
                    if (!window._maximize.interactable)
                        return false;
                    window._maximize.onClick.Invoke();
                    return true;
                }
                if (CommandManager.IsInputMatchCmd(state, windowClose))
                {
                    if (!window._close.interactable)
                        return false;
                    window._close.onClick.Invoke();
                    return true;
                }
                return false;
            }
            catch { return false; }
        }


    }
}

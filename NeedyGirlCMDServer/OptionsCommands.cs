using ngov3;
using ngov3.Effect;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

namespace NeedyGirlCMDServer
{
    internal class OptionsCommands
    {
        readonly static string[] optionLang = { "language", "lang", "l" };
        readonly static string[] optionBgm = { "music", "bgm", "m" };
        readonly static string[] optionSe = { "sound", "se", "s" };
        readonly static string[] optionWindow = { "windowsize", "window", "w" };

        static Dictionary<string, string> languages = new()
        {
            {"JP", "Japanese" },
            {"EN", "English" },
            {"CN", "Chinese (simplified)" },
            {"KO", "Korean" },
            {"TW", "Chinese (traditional)" },
            {"VN", "Vietnamese" },
            {"FR", "French" },
            {"IT", "Italian" },
            {"GE", "German" },
            {"SP", "Spanish" },
            {"RU", "Russian" }
        };

        internal static string SetOptions(string input)
        {
            var seperator = new Regex(@"\s+");
            string[] commands = seperator.Split(input, 4);
            if (commands.Length < 3)
            {
                if (SceneManager.GetActiveScene().name.Contains("Window") && SingletonMonoBehaviour<DayPassing2D>.Instance.playingAnimation)
                    return MsgManager.SendMessage(ServerMessage.CMD_BUSY);
                bool isWindowActive = SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.ControlPanel);
                if (commands.Length < 2)
                {

                    if (commands.Length == 1)
                    {
                        if (!isWindowActive)
                        {
                            if ((SceneManager.GetActiveScene().name == "BiosToLoad" && !SingletonMonoBehaviour<Boot>.Instance.Login.interactable) ||
                                (SceneManager.GetActiveScene().name != "ChooseZip" && SceneManager.GetActiveScene().name != "BiosToLoad" && !SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable))
                                return MsgManager.SendMessage(ServerMessage.CMD_SPECIFIC_BUSY);
                            SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.ControlPanel);
                        }
                        else SingletonMonoBehaviour<WindowManager>.Instance.GetWindowFromApp(AppType.ControlPanel).Touched();
                        return "";
                    }
                }
                else if (isWindowActive)
                {
                    var window = SingletonMonoBehaviour<WindowManager>.Instance.GetWindowFromApp(AppType.ControlPanel);
                    if (!(window._close.interactable || window._maximize.interactable || window._minimize.interactable))
                        return MsgManager.SendMessage(ServerMessage.OPTIONS_NO_WIN_MODIFY);
                    if (WindowCommands.IsWindowScroll(window, commands[1]))
                    {
                        return "";
                    }
                    if (!WindowCommands.ChangeWindowState(window, commands[1]))
                    {
                        return MsgManager.SendMessage(ServerMessage.OPTIONS_WIN_INVALID_CMD);
                    }
                    return "";
                }
                return MsgManager.SendMessage(ServerMessage.CMD_WRONG_ARGS);
            }
            if (CommandManager.IsInputMatchCmd(commands[1], optionLang))
            {
                return ChangeLanguage(commands[2]);
            }
            if (CommandManager.IsInputMatchCmd(commands[1], optionBgm))
            {
                return ChangeBGMVolume(commands[2]);
            }
            if (CommandManager.IsInputMatchCmd(commands[1], optionSe))
            {
                return ChangeSEVolume(commands[2]);
            }
            if (CommandManager.IsInputMatchCmd(commands[1], optionWindow))
            {
                return ChangeWindowSize(commands[2]);
            }
            else
            {
                return MsgManager.SendMessage(ServerMessage.INVALID_CMD);
            }
        }

        internal static string ChangeLanguage(string language)
        {

            language = language.ToUpper();
            switch (language)
            {
                case "JAPANESE":
                    language = "JP";
                    break;
                case "ENGLISH":
                    language = "EN";
                    break;
                case "MANDARIN":
                case "CHINESE":
                    language = "CN";
                    break;
                case "KOREAN":
                    language = "KO";
                    break;
                case "CANTONESE":
                    language = "TW";
                    break;
                case "VIETNAMESE":
                    language = "VN";
                    break;
                case "FRENCH":
                    language = "FR";
                    break;
                case "ITALIAN":
                    language = "IT";
                    break;
                case "GERMAN":
                    language = "GE";
                    break;
                case "SPANISH":
                    language = "SP";
                    break;
                case "RUSSIAN":
                    language = "RU";
                    break;
            }
            if (!Enum.TryParse<LanguageType>(language, out var selectedLang))
            {
                return MsgManager.SendMessage(ServerMessage.OPTIONS_INVALID_LANG);
            }
            SingletonMonoBehaviour<Settings>.Instance.ChangeLanguage(selectedLang);
            SingletonMonoBehaviour<Settings>.Instance.Save();
            return MsgManager.SendMessage(ServerMessage.OPTIONS_LANG_SUCCESS, languages[selectedLang.ToString()]);
        }
        internal static string ChangeBGMVolume(string volume)
        {
            float savedVolume;
            if (!int.TryParse(volume, out var num))
            {
                return MsgManager.SendMessage(ServerMessage.OPTIONS_VOL_NAN);

            }
            if (!(num > -1 && num < 101))
            {
                return MsgManager.SendMessage(ServerMessage.OPTIONS_VOL_OUTRANGE);
            }
            savedVolume = num / 100f;
            SingletonMonoBehaviour<Settings>.Instance.BgmVolume = savedVolume;
            SingletonMonoBehaviour<Settings>.Instance.Save();
            AudioManager.Instance.ChangeVolume(NGO.SoundCategory.BGM, savedVolume);
            AudioManager.Instance.ChangeVolume(NGO.SoundCategory.BANK, savedVolume);
            return MsgManager.SendMessage(ServerMessage.OPTIONS_BGM_VOL_SUCCESS, num);
        }
        internal static string ChangeSEVolume(string volume)
        {
            float savedVolume;
            if (!int.TryParse(volume, out var num))
            {
                return MsgManager.SendMessage(ServerMessage.OPTIONS_VOL_NAN);

            }
            if (!(num > -1 && num < 101))
            {
                return MsgManager.SendMessage(ServerMessage.OPTIONS_VOL_OUTRANGE);
            }
            savedVolume = num / 100f;
            SingletonMonoBehaviour<Settings>.Instance.SeVolume = savedVolume;
            SingletonMonoBehaviour<Settings>.Instance.Save();
            AudioManager.Instance.ChangeSeVolume(savedVolume);
            return MsgManager.SendMessage(ServerMessage.OPTIONS_SFX_VOL_SUCCESS, num);
        }
        internal static string ChangeWindowSize(string size)
        {
            if (size == "fullscreen")
            {
                SingletonMonoBehaviour<Settings>.Instance.Resolution.Value = ResolutionType.FullScreen;
            }
            else if (size == "windowed")
            {
                SingletonMonoBehaviour<Settings>.Instance.Resolution.Value = ResolutionType.Toubai;
            }
            else if (size == "windowedfullscreen" || size == "fullscreenwindowed")
            {
                SingletonMonoBehaviour<Settings>.Instance.Resolution.Value = ResolutionType.Window;
            }
            else
            {
                return MsgManager.SendMessage(ServerMessage.OPTIONS_SIZE_INVALID);
            }
            SingletonMonoBehaviour<Settings>.Instance.SetResolution();
            SingletonMonoBehaviour<Settings>.Instance.Save();
            return MsgManager.SendMessage(ServerMessage.OPTIONS_SIZE_SUCCESS);
        }
    }
}

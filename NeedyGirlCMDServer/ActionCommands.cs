using ngov3;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace NeedyGirlCMDServer
{
    internal class ActionCommands
    {
        readonly static string[] streamAction = { "stream", "s", "1" };
        readonly static string[] hangoutAction = { "hangout", "h", "2" };
        readonly static string[] sleepAction = { "sleep", "z", "3" };
        readonly static string[] drugAction = { "medication", "drugs", "m", "d", "4" };
        readonly static string[] onlineAction = { "internet", "i", "5" };
        readonly static string[] outsideAction = { "goout", "o", "6" };

        readonly static string[] hangoutGame = { "game", "g", "1" };
        readonly static string[] hangoutAme = { "ame", "a", "2" };
        readonly static string[] hangoutSex = { "sex", "s", "x", "3" };

        readonly static string[] sleepDusk = { "dusk", "d", "1" };
        readonly static string[] sleepNight = { "night", "n", "2" };
        readonly static string[] sleepTomorrow = { "tomorrow", "t", "3" };

        readonly static string[] drugDepazMod = { "depaz", "d", "1" };
        readonly static string[] drugDepazOver = { "depazx", "dx", "1x" };
        readonly static string[] drugDylsemMod = { "dylsem", "b", "2" };
        readonly static string[] drugDylsemOver = { "dylsemx", "bx", "2x" };
        readonly static string[] drugAmbien = { "ambien", "a", "3" };
        readonly static string[] drugWeed = { "grass", "weed", "g", "w", "4" };
        readonly static string[] drugPaper = { "paper", "lsd", "p", "l", "5" };

        readonly static string[] onlineTweet = { "tweeter", "poketter", "t", "p", "1" };
        readonly static string[] onlineSearch = { "search", "s", "2" };
        readonly static string[] onlineVideo = { "video", "v", "3" };
        readonly static string[] onlineAnon = { "anon", "st", "a", "k", "4" };
        readonly static string[] onlineDinder = { "dinder", "d", "5" };

        readonly static string[] streamChat = { "chatandchill", "chat", "c", "1" };
        readonly static string[] streamGame = { "letsplay", "game", "g", "2" };
        readonly static string[] streamNerd = { "nerdtalk", "nerd", "n", "1" };
        readonly static string[] streamConsp = { "conspiracytheories", "conspiracy", "y", "4" };
        readonly static string[] streamLore = { "netlore", "l", "5" };
        readonly static string[] streamAsmr = { "asmr", "a", "6" };
        readonly static string[] streamSexy = { "sexystream", "sexy", "x", "7" };
        readonly static string[] streamExplain = { "angelexplains", "explains", "e", "8" };
        readonly static string[] streamStuff = { "kangeltriesstuff", "stuff", "s", "9" };
        readonly static string[] streamBreak = { "breakdown", "break", "b", "10" };
        readonly static string[] streamAd = { "sponsorships", "sponsor", "ad", "p", "11" };
        readonly static string[] streamAngel = { "milestone", "internetangel", "angel", "i", "12" };

        internal static string StartAction(string input)
        {
            IWindow app = null;
            string canSpecialSex;
            string customMsg;
            Shortcut drugs = new Shortcut();
            bool isForceAction = false;
            var seperator = new Regex(@"\s+");
            string[] commands = seperator.Split(input, 5);
            var eventManager = SingletonMonoBehaviour<EventManager>.Instance;
            var windowManager = SingletonMonoBehaviour<WindowManager>.Instance;
            var horrorShortcuts = GameObject.Find("HakkyoShortCutParent").GetComponent<CanvasGroup>();
            bool isDataActive = SceneManager.GetActiveScene().name != "BiosToLoad" && SceneManager.GetActiveScene().name != "ChoozeZip";
            int dark = SingletonMonoBehaviour<StatusManager>.Instance.GetStatus(StatusType.Yami);
            int dayPart = SingletonMonoBehaviour<StatusManager>.Instance.GetStatus(StatusType.DayPart);
            if (!isDataActive)
            {
                return ErrorMessages.CMD_SPECIFIC_BUSY;
            }
            if (commands.Length < 2)
            {
                return ErrorMessages.CMD_WRONG_ARGS;
            }
            if (CommandManager.IsInputMatchCmd(commands[1], streamAction))
            {
                if (commands.Length > 2 && commands[2] == "list")
                {
                    return ListAvailableStreams();
                }
                if (dayPart > 2 && eventManager.nowEnding != NGO.EndingType.Ending_Ideon && eventManager.nowEnding != NGO.EndingType.Ending_Sucide)
                    return "It's currently too late to stream.";
                else if (dayPart < 2)
                    return "You can only stream at night.";
                return ExecuteStream(commands, eventManager, horrorShortcuts);
            }
            if ((canSpecialSex = CanExecuteSpecialSex(commands)) != "")
            {
                return canSpecialSex;
            }
            if (!SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable || (!eventManager.shortcuts.interactable && !(eventManager.isHorror && horrorShortcuts.interactable)))
            {
                if (CommandManager.IsInputMatchCmd(commands[1], outsideAction) && SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.GoOut) && !SingletonMonoBehaviour<WindowManager>.Instance.GetWindowFromApp(AppType.GoOut)._close.interactable)
                    return ExecuteGoOutAction();

                return ErrorMessages.CMD_SPECIFIC_BUSY;
            }
            if (commands.Length > 3 && (commands[3] == "f" || commands[3] == "force"))
                isForceAction = true;
            if (CommandManager.IsInputMatchCmd(commands[1], hangoutAction))
            {
                if (commands.Length == 2)
                {
                    if (eventManager.isHorror && horrorShortcuts.interactable)
                    {
                        SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.Hakkyo);
                        return "";
                    }
                    switch (windowManager.isAppOpen(AppType.Asobu))
                    {
                        case true:
                            windowManager.GetWindowFromApp(AppType.Asobu).Touched();
                            break;
                        case false:
                            windowManager.NewWindow(AppType.Asobu);
                            break;
                    }
                    return "";
                }
                if (CommandManager.IsInputMatchCmd(commands[2], hangoutGame))
                    return ExecuteAction(ActionType.EntameGame, isForceAction, horrorShortcuts);
                if (CommandManager.IsInputMatchCmd(commands[2], hangoutAme))
                    return ExecuteAction(ActionType.PlayIchatuku, isForceAction, horrorShortcuts);
                if (CommandManager.IsInputMatchCmd(commands[2], hangoutSex))
                    return ExecuteAction(ActionType.PlayMakeLove, isForceAction, horrorShortcuts);
            }
            if (CommandManager.IsInputMatchCmd(commands[1], sleepAction))
            {
                if (commands.Length == 2)
                {
                    if (eventManager.isHorror && horrorShortcuts.interactable)
                    {
                        SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.Hakkyo);
                        return "";
                    }
                    switch (windowManager.isAppOpen(AppType.Sleep))
                    {
                        case true:
                            windowManager.GetWindowFromApp(AppType.Sleep).Touched();
                            break;
                        case false:
                            windowManager.NewWindow(AppType.Sleep);
                            break;
                    }
                    return "";
                }
                if (dayPart == 0 && CommandManager.IsInputMatchCmd(commands[2], sleepDusk))
                    return ExecuteAction(ActionType.SleepToTwilight, isForceAction, horrorShortcuts);
                if (dayPart < 2 && CommandManager.IsInputMatchCmd(commands[2], sleepNight))
                    return ExecuteAction(ActionType.SleepToNight, isForceAction, horrorShortcuts);
                if (CommandManager.IsInputMatchCmd(commands[2], sleepTomorrow))
                    return ExecuteAction(ActionType.SleepToTomorrow, isForceAction, horrorShortcuts);
            }
            if (CommandManager.IsInputMatchCmd(commands[1], drugAction))
            {
                if (SingletonMonoBehaviour<NetaManager>.Instance.usedAlpha.Exists((AlphaLevel al) => al.alphaType == AlphaType.Angel && al.level == 5))
                    return "No.";
                if (commands.Length == 2)
                {
                    if (eventManager.isHorror && horrorShortcuts.interactable)
                    {
                        SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.Hakkyo);
                        return "";
                    }
                    drugs.openPillWindows();
                    return "";
                }
                if (CommandManager.IsInputMatchCmd(commands[2], drugDepazMod))
                    return ExecuteAction(ActionType.OkusuriDaypassModerate, isForceAction, horrorShortcuts);
                if (CommandManager.IsInputMatchCmd(commands[2], drugDepazOver))
                    return ExecuteAction(ActionType.OkusuriDaypassOverdose, isForceAction, horrorShortcuts);
                if (dark >= 20 && CommandManager.IsInputMatchCmd(commands[2], drugDylsemMod))
                    return ExecuteAction(ActionType.OkusuriPuronModerate, isForceAction, horrorShortcuts);
                if (dark >= 20 && CommandManager.IsInputMatchCmd(commands[2], drugDylsemOver))
                    return ExecuteAction(ActionType.OkusuriPuronOverdose, isForceAction, horrorShortcuts);
                if (dark >= 40 && CommandManager.IsInputMatchCmd(commands[2], drugAmbien))
                    return ExecuteAction(ActionType.OkusuriHiPuronOverdose, isForceAction, horrorShortcuts);
                if (dark >= 60 && CommandManager.IsInputMatchCmd(commands[2], drugWeed))
                    return ExecuteAction(ActionType.OkusuriHappa, isForceAction, horrorShortcuts);
                if (dark >= 80 && CommandManager.IsInputMatchCmd(commands[2], drugPaper))
                    return ExecuteAction(ActionType.OkusuriPsyche, isForceAction, horrorShortcuts);
            }
            if (CommandManager.IsInputMatchCmd(commands[1], onlineAction))
            {
                if (commands.Length == 2)
                {
                    if (eventManager.isHorror && horrorShortcuts.interactable)
                    {
                        SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.Hakkyo);
                        return "";
                    }
                    switch (windowManager.isAppOpen(AppType.Internet))
                    {
                        case true:
                            windowManager.GetWindowFromApp(AppType.Internet).Touched();
                            break;
                        case false:
                            windowManager.NewWindow(AppType.Internet);
                            break;
                    }
                    return "";
                }
                if (CommandManager.IsInputMatchCmd(commands[2], onlineTweet))
                    return ExecuteAction(ActionType.InternetPoketter, isForceAction, horrorShortcuts);
                if (CommandManager.IsInputMatchCmd(commands[2], onlineSearch))
                    return ExecuteAction(ActionType.InternetPoketterEgosa, isForceAction, horrorShortcuts);
                if (CommandManager.IsInputMatchCmd(commands[2], onlineVideo))
                    return ExecuteAction(ActionType.InternetYoutube, isForceAction, horrorShortcuts);
                if (CommandManager.IsInputMatchCmd(commands[2], onlineAnon))
                {
                    if (!SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.Keijiban))
                        app = SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.Keijiban);
                    if (commands.Length > 3 && !isForceAction)
                    {
                        if (commands.Length == 5)
                            customMsg = string.Join(" ", commands[3], commands[4]);
                        else customMsg = commands[3];
                        app.nakamiApp.GetComponentInChildren<KitsuneView>().input.text = customMsg.TrimStart();
                    }
                    else if (commands.Length == 5 && isForceAction)
                    {
                        app.nakamiApp.GetComponentInChildren<KitsuneView>().input.text = commands[4].TrimStart();
                    }
                    return ExecuteAction(ActionType.Internet2ch, isForceAction, horrorShortcuts);
                }

                if (CommandManager.IsInputMatchCmd(commands[2], onlineDinder))
                {
                    if (!SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.Dinder))
                        SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.Dinder);
                    return ExecuteAction(ActionType.InternetDeai, isForceAction, horrorShortcuts);
                }
            }
            if (CommandManager.IsInputMatchCmd(commands[1], outsideAction))
            {
                return ExecuteGoOutAction();
            }
            return "Invalid action.";


            string ExecuteGoOutAction()
            {
                if (commands.Length == 2)
                {
                    if (eventManager.isHorror && horrorShortcuts.interactable)
                    {
                        SingletonMonoBehaviour<ngov3.WindowManager>.Instance.NewWindow(AppType.Hakkyo);
                        return "";
                    }
                    switch (windowManager.isAppOpen(AppType.GoOut))
                    {
                        case true:
                            windowManager.GetWindowFromApp(AppType.GoOut).Touched();
                            break;
                        case false:
                            windowManager.NewWindow(AppType.GoOut);
                            break;
                    }
                    return "";
                }
                switch (commands[2])
                {
                    case "hospital":
                        return ExecuteAction(ActionType.OdekakeHospital, isForceAction, horrorShortcuts);

                    case "kichijoji":
                        return ExecuteAction(ActionType.OdekakeKichijoji, isForceAction, horrorShortcuts);

                    case "hikarigaokapark":
                    case "park":
                        return ExecuteAction(ActionType.OdekakeHikarigaokaPark, isForceAction, horrorShortcuts);

                    case "ikebukuro":
                        return ExecuteAction(ActionType.OdekakeIkebukuro, isForceAction, horrorShortcuts);

                    case "nakano":
                        return ExecuteAction(ActionType.OdekakeNakano, isForceAction, horrorShortcuts);

                    case "shimokitazawa":
                        return ExecuteAction(ActionType.OdekakeShimokitazawa, isForceAction, horrorShortcuts);

                    case "shinjuku":
                        return ExecuteAction(ActionType.OdekakeShinjuku, isForceAction, horrorShortcuts);

                    case "ueno":
                        return ExecuteAction(ActionType.OdekakeUeno, isForceAction, horrorShortcuts);

                    case "harajuku":
                        return ExecuteAction(ActionType.OdekakeHarajuku, isForceAction, horrorShortcuts);

                    case "ichigaya":
                        return ExecuteAction(ActionType.OdekakeIchigaya, isForceAction, horrorShortcuts);

                    case "jinbocho":
                        return ExecuteAction(ActionType.OdekakeJinbocho, isForceAction, horrorShortcuts);

                    case "shibuya":
                        return ExecuteAction(ActionType.OdekakeShibuya, isForceAction, horrorShortcuts);

                    case "akihabara":
                        return ExecuteAction(ActionType.OdekakeAkihabara, isForceAction, horrorShortcuts);

                    case "asakusa":
                        return ExecuteAction(ActionType.OdekakeAsakusa, isForceAction, horrorShortcuts);

                    case "gisneyland":
                    case "disney":
                        return ExecuteAction(ActionType.OdekakeGisneyland, isForceAction, horrorShortcuts);

                    case "toyosu":
                        return ExecuteAction(ActionType.OdekakeToyosu, isForceAction, horrorShortcuts);

                }
                if (eventManager.isOpenGinga && SingletonMonoBehaviour<StatusManager>.Instance.isTodayGangimari && commands[2] == "galacticrail")
                    return ExecuteAction(ActionType.OdekakeGinga, isForceAction, horrorShortcuts);
                return "Invalid action.";
            }

        }


        internal static string CanExecuteSpecialSex(string[] commands)
        {
            var eventManager = SingletonMonoBehaviour<EventManager>.Instance;
            var windowManager = SingletonMonoBehaviour<WindowManager>.Instance;
            if (!windowManager.isAppOpen(AppType.AsobuNeedy) && !windowManager.isAppOpen(AppType.AsobuLust))
                return "";
            if (CommandManager.IsInputMatchCmd(commands[1], hangoutAction))
            {
                if (CommandManager.IsInputMatchCmd(commands[2], hangoutSex))
                {
                    ExecuteSpecialSex(windowManager, eventManager);
                    return "<3";
                }
            }
            return ErrorMessages.CMD_SPECIFIC_BUSY;
        }

        static void ExecuteSpecialSex(WindowManager windowManager, EventManager eventManager)
        {
            if (windowManager.isAppOpen(AppType.AsobuNeedy))
            {
                eventManager.AddEvent<Action_Needy>();
            }
            else if (windowManager.isAppOpen(AppType.AsobuLust))
            {
                eventManager.AddEvent<Action_Inran>();
            }
        }

        internal static string ExecuteAction(ActionType action, bool isForced, CanvasGroup horrorShortcuts)
        {
            var eventManager = SingletonMonoBehaviour<EventManager>.Instance;
            var cmdType = SingletonMonoBehaviour<ngov3.CommandManager>.Instance.ChooseCommand(action);
            if (eventManager.isHorror && horrorShortcuts.interactable)
            {
                SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.Hakkyo);
                return "";
            }
            if (isForced)
            {
                eventManager.ExecuteActionConfirmed(action, cmdType);
                return "";
            }
            eventManager.ExecuteAction(action, cmdType);
            return "";
        }

        internal static string ExecuteStream(string[] commands, EventManager eventManager, CanvasGroup horrorShortcuts)
        {
            int level;
            AlphaType streamTopic;
            var netaManager = SingletonMonoBehaviour<NetaManager>.Instance;
            var windowManager = SingletonMonoBehaviour<WindowManager>.Instance;
            if (commands.Length == 2)
            {
                if (eventManager.nowEnding == NGO.EndingType.Ending_Sucide)
                {
                    var obj = GameObject.Find("MainPanel").transform.Find("liveend").gameObject.activeInHierarchy;
                    if (obj)
                    {
                        SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.sayonalastDialog);
                        return "...";
                    }
                }
                else if (eventManager.isGedatsu)
                {
                    eventManager.nowEnding = NGO.EndingType.Ending_Kyouso;
                    eventManager.AddEvent<Action_HaishinStart>();
                    return "";
                }
                else if (eventManager.nowEnding == NGO.EndingType.Ending_Ideon && SingletonMonoBehaviour<WindowManager>.Instance.TaskBarList.Exists(t => t.window.appType == AppType.Ideon_taiki))
                {
                    eventManager.AddEvent<HaishinStart_Ideon>();
                    return "";
                }
                else if (eventManager.isHorror && horrorShortcuts.interactable)
                {
                    SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.Hakkyo);
                    return "";
                }
                else
                {
                    switch (windowManager.isAppOpen(AppType.NetaChoose))
                    {
                        case true:
                            windowManager.GetWindowFromApp(AppType.NetaChoose).Touched();
                            break;
                        case false:
                            windowManager.NewWindow(AppType.NetaChoose);
                            break;
                    }
                }
                return "";
            }
            if (eventManager.isHorror && horrorShortcuts.interactable)
            {
                SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.Hakkyo);
                return "";
            }
            if (!SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable || !eventManager.shortcuts.interactable)
            {
                return ErrorMessages.CMD_SPECIFIC_BUSY;
            }
            if (eventManager.nowEnding != NGO.EndingType.Ending_None)
            {
                return "Can't stream anything specific now.";
            }
            streamTopic = GetStreamTopic(commands[2]);
            if (streamTopic == AlphaType.none)
            {
                return "Invalid stream topic.";
            }
            if (!TryGetStreamLevel(netaManager, streamTopic, out level))
            {
                return "No new stream for this topic exists.";
            }
            var usedStream = netaManager.usedAlpha.Find(s => s.alphaType == streamTopic && s.level == level);
            var gotStream = netaManager.GotAlpha.Find(s => s.alphaType == streamTopic && s.level == level);
            var highestStream = netaManager.GotAlpha.FindLast(s => s.alphaType == streamTopic);
            if (eventManager.isGedatsu)
            {
                eventManager.nowEnding = NGO.EndingType.Ending_Kyouso;
                eventManager.AddEvent<Action_HaishinStart>();
                return "";
            }
            if (usedStream != null)
            {
                return "This stream has already been done.";
            }
            if (streamTopic == AlphaType.Angel && level == 5 && IsDarkAngelPossible(netaManager))
            {
                level = 6;
            }
            else if (gotStream == null)
            {
                return "This stream has not been found yet.";
            }
            if (streamTopic == AlphaType.Angel && usedStream == null && highestStream.level > level)
            {
                return "This isn't the latest milestone stream!";
            }
            SingletonMonoBehaviour<EventManager>.Instance.StartHaishin(streamTopic, level, BetaType.none);
            return "";
        }

        static bool IsDarkAngelPossible(NetaManager netaManager)
        {
            var usedNormalStream = netaManager.usedAlpha.Find(s => s.alphaType == AlphaType.Angel && s.level == 5);
            var gotDarkStream = netaManager.GotAlpha.Find(s => s.alphaType == AlphaType.Angel && s.level == 6);
            if (usedNormalStream == null && gotDarkStream != null)
            {
                return true;
            }
            return false;
        }

        static string ListAvailableStreams(NetaManager netaManager = null)
        {
            AlphaTypeToData streamData;
            string streamList = "\n";
            var lang = SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value;
            if (netaManager == null)
            {
                netaManager = SingletonMonoBehaviour<NetaManager>.Instance;
            }
            for (int i = 0; i < 12; i++)
            {
                var gotStream = netaManager.GotAlpha.FindLast(s => s.alphaType == (AlphaType)i);
                var usedStream = netaManager.usedAlpha.Find(s => s.alphaType == (AlphaType)i && s.level == gotStream.level);

                if (gotStream != null && usedStream == null)
                {
                    streamData = LoadNetaData.ReadNetaContent(gotStream.alphaType, gotStream.level);
                    streamList += $"{NgoEx.CmdName(streamData.netaGenre, lang)}\n";
                }
                if (gotStream?.alphaType == AlphaType.Imbouron && gotStream?.level == 6)
                {
                    streamList = "?????";
                    break;
                }
            }
            return streamList;
        }

        static AlphaType GetStreamTopic(string topic)
        {
            if (CommandManager.IsInputMatchCmd(topic, streamChat))
                return AlphaType.Zatudan;
            if (CommandManager.IsInputMatchCmd(topic, streamGame))
                return AlphaType.Gamejikkyou;
            if (CommandManager.IsInputMatchCmd(topic, streamNerd))
                return AlphaType.Otakutalk;
            if (CommandManager.IsInputMatchCmd(topic, streamConsp))
                return AlphaType.Imbouron;
            if (CommandManager.IsInputMatchCmd(topic, streamLore))
                return AlphaType.Kaidan;
            if (CommandManager.IsInputMatchCmd(topic, streamAsmr))
                return AlphaType.ASMR;
            if (CommandManager.IsInputMatchCmd(topic, streamSexy))
                return AlphaType.Hnahaisin;
            if (CommandManager.IsInputMatchCmd(topic, streamExplain))
                return AlphaType.Kaisetu;
            if (CommandManager.IsInputMatchCmd(topic, streamStuff))
                return AlphaType.Taiken;
            if (CommandManager.IsInputMatchCmd(topic, streamBreak))
                return AlphaType.Yamihaishin;
            if (CommandManager.IsInputMatchCmd(topic, streamAd))
                return AlphaType.PR;
            if (CommandManager.IsInputMatchCmd(topic, streamAngel))
                return AlphaType.Angel;
            return AlphaType.none;
        }

        static bool TryGetStreamLevel(NetaManager netaManager, AlphaType streamTopic, out int streamLevel)
        {
            streamLevel = 0;
            var usedStream = netaManager.usedAlpha.FindLast(s => s.alphaType == streamTopic);
            var gotStream = netaManager.GotAlpha.FindLast(s => s.alphaType == streamTopic);
            if (gotStream == null)
                return false;
            if (usedStream != null && usedStream.level >= gotStream.level)
                return false;
            streamLevel = gotStream.level;
            return true;
        }
    }
}

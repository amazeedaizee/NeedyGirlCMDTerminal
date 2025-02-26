using NGO;
using ngov3;
using System;
using UnityEngine.SceneManagement;

namespace NeedyGirlCMDServer
{
    internal class InfoCommands
    {
        internal static string ShowInfo()
        {
            string info = "\n";
            string ngoEvent;
            string stream;
            bool isDataActive = SceneManager.GetActiveScene().name != "BiosToLoad" && SceneManager.GetActiveScene().name != "ChoozeZip";
            if (!isDataActive)
            {
                return MsgManager.SendMessage(ServerMessage.CMD_SPECIFIC_BUSY);
            }

            if (SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable && SingletonMonoBehaviour<EventManager>.Instance.nowEnding != EndingType.Ending_Completed)
            {
                bool isWindowActive = SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.TaskManager);
                if (!isWindowActive)
                {
                    SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.TaskManager);
                }
                else SingletonMonoBehaviour<WindowManager>.Instance.GetWindowFromApp(AppType.TaskManager).Touched();
            }

            var statusManager = SingletonMonoBehaviour<StatusManager>.Instance;
            var eventManager = SingletonMonoBehaviour<EventManager>.Instance;
            var lang = SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value;

            var day = statusManager.GetStatus(StatusType.DayIndex);
            var dayPart = NgoEx.TimeText(statusManager.GetStatus(StatusType.DayPart), lang);
            var dayEvent = eventManager.eventsHistory.FindLast(e => e.Contains("_"));

            var followers = statusManager.GetStatus(StatusType.Follower);
            var stress = statusManager.GetStatus(StatusType.Stress);
            var love = statusManager.GetStatus(StatusType.Love);
            var darkness = statusManager.GetStatus(StatusType.Yami);

            var firstDate = NgoEx.CmdName(eventManager.FirstDate, lang);
            var lastAction = (ngoEvent = eventManager.dayActionHistory.FindLast(a => Enum.TryParse<CmdType>(a, out _))) != null ?
                              NgoEx.CmdName((CmdType)Enum.Parse(typeof(CmdType), ngoEvent), lang) : "None";
            var lastStream = (stream = eventManager.dayActionHistory.FindLast(a => a.Contains("_") && Enum.TryParse<CmdType>(a, out _))) != null ?
                              NgoEx.CmdName((CmdType)Enum.Parse(typeof(CmdType), stream), lang) :
                              "None";

            var ignoreCount = eventManager.midokumushi;
            var sexCount = statusManager.GetStatus(StatusType.MadeLoveCounter);
            var paperCount = eventManager.psycheCount;
            var breakCount = eventManager.kyuusiCount;
            var hasSeenLight = eventManager.isGedatsu;
            var hasGalaxyIdea = eventManager.isOpenGinga;

            var stressState = SetStressPoint(eventManager, statusManager);

            info += $"INFO:\n\n" +

                    $"DAY: {day}\n" +
                    $"DAY PART: {dayPart}\n" +
                    $"DAY EVENT: {(string.IsNullOrWhiteSpace(dayEvent) ? "None" : dayEvent)}\n\n" +

                    $"FOLLOWERS: {followers}\n" +
                    $"STRESS: {stress}\n" +
                    $"AFFECTION: {love}\n" +
                    $"DARKNESS: {darkness}\n\n" +

                    $"FIRST DATE LOCATION: {firstDate}\n" +
                    $"TRAUMA: {SetTrauma(eventManager)}\n\n" +

                    $"LAST ACTION: {(string.IsNullOrWhiteSpace(lastAction) ? "None" : lastAction)}\n" +
                    $"LAST STREAM: {(string.IsNullOrWhiteSpace(lastStream) ? "None" : lastStream)}\n\n" +

                    $"IGNORE COUNT: {ignoreCount}\n" +
                    $"*** COUNT: {sexCount}\n" +
                    $"PAPER COUNT: {paperCount}\n";

            if (breakCount > 0 || hasSeenLight || hasSeenLight || stressState != "")
            {
                info += "\n";
            }
            if (stressState != "")
                info += $"{stressState}\n";
            if (breakCount > 0)
                info += MsgManager.SendMessage(ServerMessage.INFO_HIATUS);
            if (hasSeenLight)
            {
                info += MsgManager.SendMessage(ServerMessage.INFO_RELIGION);
            }
            if (hasGalaxyIdea)
            {
                info += MsgManager.SendMessage(ServerMessage.INFO_GALACTIC_RAIL);
            }

            return info;
        }

        static string SetStressPoint(EventManager eventManager, StatusManager statusManager)
        {
            if (eventManager.isHorror && statusManager.GetStatus(StatusType.DayIndex) > 27)
                return MsgManager.SendMessage(ServerMessage.INFO_STRESS_HORROR);
            if (eventManager.isHakkyo)
                return MsgManager.SendMessage(ServerMessage.INFO_STRESS_BREAK);
            if (eventManager.isWristCut)
                return MsgManager.SendMessage(ServerMessage.INFO_STRESS_CUT);
            if (eventManager.beforeWristCut)
                return MsgManager.SendMessage(ServerMessage.INFO_STRESS_BEFORE);
            return "";
        }

        static string SetTrauma(EventManager eventManager)
        {
            switch (eventManager.Trauma)
            {
                case NGO.JineType.Event_LongLINE001:
                    return MsgManager.SendMessage(ServerMessage.INFO_TRAUMA_1);
                case NGO.JineType.Event_LongLINE002:
                    return MsgManager.SendMessage(ServerMessage.INFO_TRAUMA_2);
                case NGO.JineType.Event_LongLINE003:
                    return MsgManager.SendMessage(ServerMessage.INFO_TRAUMA_3);
                case NGO.JineType.Event_LongLINE004:
                    return MsgManager.SendMessage(ServerMessage.INFO_TRAUMA_4);
                case NGO.JineType.Event_LongLINE005:
                    return MsgManager.SendMessage(ServerMessage.INFO_TRAUMA_5);
                default:
                    return MsgManager.SendMessage(ServerMessage.INFO_TRAUMA_NONE);
            }
        }

        internal static string ShowEndingInfo()
        {
            string endingInfo = "";
            EndingType end;
            bool isDataActive = SceneManager.GetActiveScene().name != "BiosToLoad" && SceneManager.GetActiveScene().name != "ChoozeZip";
            if (!isDataActive)
            {
                return MsgManager.SendMessage(ServerMessage.CMD_SPECIFIC_BUSY);
            }
            var eventManager = SingletonMonoBehaviour<EventManager>.Instance;
            var lang = MsgManager.currentLang;
            end = eventManager.nowEnding;
            if (eventManager.nowEnding == NGO.EndingType.Ending_None)
            {
                return MsgManager.SendMessage(ServerMessage.INFO_ENDING_NONE);
            }
            endingInfo += $"{GetEndingTitle(end, lang)}\n\n" +
                         $"{GetEndingDesc(end, lang)}\n\n" +
                         $"\"{NgoEx.ReasonTextFromEDType(end, lang)}\"\n";
            return endingInfo;
        }

        internal static string ShowEndingListInfo()
        {
            string endingInfo = "\n";
            var lang = MsgManager.currentLang;
            var endings = Enum.GetValues(typeof(EndingType));
            var gotEnds = SingletonMonoBehaviour<Settings>.Instance.mitaEnd;
            foreach (EndingType e in endings)
            {
                bool hasEnd = false;
                if (e == EndingType.Ending_None || e == EndingType.Ending_NetShut || e == EndingType.Ending_Completed)
                    continue;
                if (gotEnds.Exists(end => end == e))
                {
                    hasEnd = true;
                }
                endingInfo += $"{GetEndingTitle(e, lang)}: {(hasEnd ? 'O' : 'X')}\n";
            }
            return endingInfo;
        }

        static string GetEndingTitle(EndingType end, LanguageType lang)
        {
            switch (lang)
            {
                case LanguageType.JP:
                    return NgoEx.EndingFromType(end).EndingNameJp;
                case LanguageType.CN:
                    return NgoEx.EndingFromType(end).EndingNameCn;
                case LanguageType.KO:
                    return NgoEx.EndingFromType(end).EndingNameKo;
                case LanguageType.TW:
                    return NgoEx.EndingFromType(end).EndingNameTw;
                case LanguageType.VN:
                    return NgoEx.EndingFromType(end).EndingNameVn;
                case LanguageType.FR:
                    return NgoEx.EndingFromType(end).EndingNameFr;
                case LanguageType.IT:
                    return NgoEx.EndingFromType(end).EndingNameIt;
                case LanguageType.GE:
                    return NgoEx.EndingFromType(end).EndingNameGe;
                case LanguageType.SP:
                    return NgoEx.EndingFromType(end).EndingNameSp;
                case LanguageType.RU:
                    return NgoEx.EndingFromType(end).EndingNameRu;
                default:
                    return NgoEx.EndingFromType(end).EndingNameEn;
            }
        }

        static string GetEndingDesc(EndingType end, LanguageType lang)
        {
            switch (lang)
            {
                case LanguageType.JP:
                    return NgoEx.EndingFromType(end).JissekiJp;
                case LanguageType.CN:
                    return NgoEx.EndingFromType(end).JissekiCn;
                case LanguageType.KO:
                    return NgoEx.EndingFromType(end).JissekiKo;
                case LanguageType.TW:
                    return NgoEx.EndingFromType(end).JissekiTw;
                case LanguageType.VN:
                    return NgoEx.EndingFromType(end).JissekiVn;
                case LanguageType.FR:
                    return NgoEx.EndingFromType(end).JissekiFr;
                case LanguageType.IT:
                    return NgoEx.EndingFromType(end).JissekiIt;
                case LanguageType.GE:
                    return NgoEx.EndingFromType(end).JissekiGe;
                case LanguageType.SP:
                    return NgoEx.EndingFromType(end).JissekiSp;
                case LanguageType.RU:
                    return NgoEx.EndingFromType(end).JissekiRu;
                default:
                    return NgoEx.EndingFromType(end).JissekiEn;
            }
        }
    }
}

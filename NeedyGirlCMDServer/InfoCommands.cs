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
                return ErrorMessages.CMD_SPECIFIC_BUSY;
            }

            if (!SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable)
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
                info += "Ame is currently on hiatus.\n";
            if (hasSeenLight)
            {
                info += "Ame has seen the light.\n";
            }
            if (hasGalaxyIdea)
            {
                info += "Ame likes trains.\n";
            }

            return info;
        }

        static string SetStressPoint(EventManager eventManager, StatusManager statusManager)
        {
            if (eventManager.isHorror && statusManager.GetStatus(StatusType.DayIndex) > 28)
                return "Ame has completely lost it.";
            if (eventManager.isHakkyo)
                return "Ame is currently on edge.";
            if (eventManager.isWristCut)
                return "Ame is self-destructing.";
            if (eventManager.beforeWristCut)
                return "Ame is close to breaking!";
            return "";
        }

        static string SetTrauma(EventManager eventManager)
        {
            switch (eventManager.Trauma)
            {
                case NGO.JineType.Event_LongLINE001:
                    return "Parents";
                case NGO.JineType.Event_LongLINE002:
                    return "Poverty";
                case NGO.JineType.Event_LongLINE003:
                    return "School";
                case NGO.JineType.Event_LongLINE004:
                    return "Marriage";
                case NGO.JineType.Event_LongLINE005:
                    return "Health";
                default:
                    return "Nothing (she is fine)";
            }
        }

        internal static string ShowEndingInfo()
        {
            string endingInfo = "";
            EndingType end;
            var eventManager = SingletonMonoBehaviour<EventManager>.Instance;
            var lang = SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value;
            end = eventManager.nowEnding;
            if (eventManager.nowEnding == NGO.EndingType.Ending_None)
            {
                return "No ending is happening!";
            }
            endingInfo += $"{GetEndingTitle(end, lang)}\n\n" +
                         $"{GetEndingDesc(end, lang)}\n\n" +
                         $"\"{NgoEx.ReasonTextFromEDType(end, lang)}\"\n";
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

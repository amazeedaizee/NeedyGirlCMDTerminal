using ngov3;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NeedyGirlCMDServer
{
    internal class TextReaderCommands
    {
        const string TEXT_NOT_FOUND = "No such file was found.";
        const string NO_PERMISSIONS = "You do not have permission to open this file.";
        const string ENDING_HINT = "netshuthint";
        internal static string OpenTextDoc(string input)
        {
            var seperator = new Regex(@"\s+");
            string[] commands = seperator.Split(input, 4);
            string title = commands[1];
            var eventManager = SingletonMonoBehaviour<EventManager>.Instance;
            bool isDataActive = SceneManager.GetActiveScene().name != "BiosToLoad" && SceneManager.GetActiveScene().name != "ChoozeZip";
            if (!isDataActive)
            {
                return MsgManager.INVALID_CMD;
            }
            if (eventManager.isTestScene)
            {
                return OpenText_Debug(commands);
            }
            if (ValidDiaryTitle(title))
            {
                switch (eventManager.loveDiary)
                {
                    case 1:
                        eventManager.LoveNikki4.GetComponent<Shortcut>()._shortcut.onClick.Invoke();
                        break;
                    case 2:
                        eventManager.LoveNikki3.GetComponent<Shortcut>()._shortcut.onClick.Invoke();
                        break;
                    case 3:
                        eventManager.LoveNikki2.GetComponent<Shortcut>()._shortcut.onClick.Invoke();
                        break;
                    case 4:
                        eventManager.LoveNikki1.GetComponent<Shortcut>()._shortcut.onClick.Invoke();
                        break;
                    default:
                        return TEXT_NOT_FOUND;
                }
            }
            else if (ValidSecretTitle(title))
            {
                return OpenSecret();
            }
            else if (eventManager.psycheCount != 0)
            {

                if (eventManager.psycheCount >= 1 && ValidLogOneTitle(title))
                    eventManager.PsycheNikki1.GetComponent<Shortcut>()._shortcut.onClick.Invoke();
                else if (eventManager.psycheCount >= 2 && ValidLogTwoTitle(title))
                    eventManager.PsycheNikki2.GetComponent<Shortcut>()._shortcut.onClick.Invoke();
                else if (eventManager.psycheCount >= 3 && ValidLogThreeTitle(title))
                    eventManager.PsycheNikki3.GetComponent<Shortcut>()._shortcut.onClick.Invoke();
                else if (eventManager.psycheCount >= 4 && ValidLogFourTitle(title))
                    eventManager.PsycheNikki4.GetComponent<Shortcut>()._shortcut.onClick.Invoke();
                else return TEXT_NOT_FOUND;

            }
            else return TEXT_NOT_FOUND;
            return "";
        }
        internal static string ReadTextDoc(string input)
        {
            char[] seperator = { ' ' };
            string[] commands = input.Split(seperator, 4);
            string title = commands[1];
            var eventManager = SingletonMonoBehaviour<EventManager>.Instance;
            var lang = SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value;
            if (eventManager.isTestScene)
            {
                return ReadDoc_Debug(commands);
            }
            if (ValidDiaryTitle(title))
            {
                if (eventManager.isTestScene)
                {
                    if (commands.Length < 3)
                    {
                        return "Opening this file while debugging must require a number.";
                    }
                    if (!int.TryParse(commands[2], out int level))
                    {
                        return "Not a valid number.";
                    }
                    switch (level)
                    {
                        case 1:
                            return NgoEx.EventTextTypeToText(NGO.EventTextType.Event_Diary004, lang);
                        case 2:
                            return NgoEx.EventTextTypeToText(NGO.EventTextType.Event_Diary003, lang);
                        case 3:
                            return NgoEx.EventTextTypeToText(NGO.EventTextType.Event_Diary002, lang);
                        case 4:
                            return NgoEx.EventTextTypeToText(NGO.EventTextType.Event_Diary001, lang);
                        default:
                            return TEXT_NOT_FOUND;
                    }
                }
                switch (eventManager.loveDiary)
                {
                    case 1:
                        return NgoEx.EventTextTypeToText(NGO.EventTextType.Event_Diary004, lang);
                    case 2:
                        return NgoEx.EventTextTypeToText(NGO.EventTextType.Event_Diary003, lang);
                    case 3:
                        return NgoEx.EventTextTypeToText(NGO.EventTextType.Event_Diary002, lang);
                    case 4:
                        return NgoEx.EventTextTypeToText(NGO.EventTextType.Event_Diary001, lang);
                    default:
                        return TEXT_NOT_FOUND;
                }
            }
            else if (ValidSecretTitle(title))
            {
                return ReadSecret(lang);
            }
            else if (eventManager.psycheCount != 0)
            {

                if (eventManager.psycheCount >= 1 && ValidLogOneTitle(title))
                    return NgoEx.EventTextTypeToText(NGO.EventTextType.Event_Psyche_Nikki_1, lang);
                else if (eventManager.psycheCount >= 2 && ValidLogTwoTitle(title))
                    return NgoEx.EventTextTypeToText(NGO.EventTextType.Event_Psyche_Nikki_2, lang);
                else if (eventManager.psycheCount >= 3 && ValidLogThreeTitle(title))
                    return NgoEx.EventTextTypeToText(NGO.EventTextType.Event_Psyche_Nikki_3, lang);
                else if (eventManager.psycheCount >= 4 && ValidLogFourTitle(title))
                    return NgoEx.EventTextTypeToText(NGO.EventTextType.Event_Psyche_Nikki_4, lang);
                else return TEXT_NOT_FOUND;

            }
            else return TEXT_NOT_FOUND;
        }

        static string OpenText_Debug(string[] commands)
        {
            string title = commands[1];
            var eventManager = SingletonMonoBehaviour<EventManager>.Instance;
            var windowManager = SingletonMonoBehaviour<WindowManager>.Instance;
            if (ValidDiaryTitle(title))
            {
                if (commands.Length < 3)
                {
                    return "Opening this file while debugging must require a number.";
                }
                if (!int.TryParse(commands[2], out int level))
                {
                    return "Not a valid number.";
                }
                switch (level)
                {
                    case 1:
                        windowManager.NewWindow(AppType.Event_Diary004);
                        break;
                    case 2:
                        windowManager.NewWindow(AppType.Event_Diary003);
                        break;
                    case 3:
                        windowManager.NewWindow(AppType.Event_Diary002);
                        break;
                    case 4:
                        windowManager.NewWindow(AppType.Event_Diary001);
                        break;
                    default:
                        return TEXT_NOT_FOUND;
                }
                return "";
            }
            else if (ValidSecretTitle(title))
            {
                return OpenSecret();
            }
            else if (title == ENDING_HINT)
            {
                return SelectNetShutHint(true);
            }
            else if (ValidLogOneTitle(title))
                eventManager.PsycheNikki1.GetComponent<Shortcut>()._shortcut.onClick.Invoke();
            else if (ValidLogTwoTitle(title))
                eventManager.PsycheNikki2.GetComponent<Shortcut>()._shortcut.onClick.Invoke();
            else if (ValidLogThreeTitle(title))
                eventManager.PsycheNikki3.GetComponent<Shortcut>()._shortcut.onClick.Invoke();
            else if (ValidLogFourTitle(title))
                eventManager.PsycheNikki4.GetComponent<Shortcut>()._shortcut.onClick.Invoke();
            else return TEXT_NOT_FOUND;
            return "";
        }

        static string ReadDoc_Debug(string[] commands)
        {
            string title = commands[1];
            var lang = SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value;
            if (ValidDiaryTitle(title))
            {
                if (commands.Length < 3)
                {
                    return "Opening this file while debugging must require a number.";
                }
                if (!int.TryParse(commands[2], out int level))
                {
                    return "Not a valid number.";
                }
                switch (level)
                {
                    case 1:
                        return NgoEx.EventTextTypeToText(NGO.EventTextType.Event_Diary001, lang);
                    case 2:
                        return NgoEx.EventTextTypeToText(NGO.EventTextType.Event_Diary002, lang);
                    case 3:
                        return NgoEx.EventTextTypeToText(NGO.EventTextType.Event_Diary003, lang);
                    case 4:
                        return NgoEx.EventTextTypeToText(NGO.EventTextType.Event_Diary004, lang);
                    default:
                        return TEXT_NOT_FOUND;
                }
            }
            else if (ValidSecretTitle(title))
            {
                return ReadSecret(lang);
            }
            else if (title == ENDING_HINT)
            {
                return SelectNetShutHint(false, lang);
            }
            else if (ValidLogOneTitle(title))
                return NgoEx.EventTextTypeToText(NGO.EventTextType.Event_Psyche_Nikki_1, lang);
            else if (ValidLogTwoTitle(title))
                return NgoEx.EventTextTypeToText(NGO.EventTextType.Event_Psyche_Nikki_2, lang);
            else if (ValidLogThreeTitle(title))
                return NgoEx.EventTextTypeToText(NGO.EventTextType.Event_Psyche_Nikki_3, lang);
            else if (ValidLogFourTitle(title))
                return NgoEx.EventTextTypeToText(NGO.EventTextType.Event_Psyche_Nikki_4, lang);
            else return TEXT_NOT_FOUND;
        }

        static string OpenSecret()
        {
            GameObject secret = GameObject.Find("ShortCutParent").transform.Find("himitu").gameObject;
            GameObject otherSecret = GameObject.Find("himitu");
            if (otherSecret.GetComponent<EventKakusinikuru>().enabled)
                return NO_PERMISSIONS;
            secret.GetComponent<Shortcut>()._shortcut.onClick.Invoke();
            return "";
        }

        static string ReadSecret(LanguageType lang)
        {
            GameObject secret = GameObject.Find("ShortCutParent").transform.Find("himitu").gameObject;
            GameObject otherSecret = GameObject.Find("himitu");
            if (otherSecret.GetComponent<EventKakusinikuru>().enabled)
                return NO_PERMISSIONS;
            secret.GetComponent<Shortcut>()._shortcut.onClick.Invoke();
            return NgoEx.SystemTextFromType(NGO.SystemTextType.System_secret_Contents, lang);
        }

        static string SelectNetShutHint(bool openWindow, LanguageType lang = LanguageType.EN)
        {
            if (openWindow)
            {
                SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.HintTextAfterEnding);
                return "";
            }
            else return NgoEx.SystemTextFromType(NGO.SystemTextType.NetShutHint, lang);
        }

        static bool ValidDiaryTitle(string title)
        {
            string diaryJP = NgoEx.SystemTextFromTypeString("Shortcut_nikki", LanguageType.JP);
            string diaryEN = NgoEx.SystemTextFromTypeString("Shortcut_nikki", LanguageType.EN);
            string diaryCN = NgoEx.SystemTextFromTypeString("Shortcut_nikki", LanguageType.CN);
            string diaryKO = NgoEx.SystemTextFromTypeString("Shortcut_nikki", LanguageType.KO);
            string diaryTW = NgoEx.SystemTextFromTypeString("Shortcut_nikki", LanguageType.TW);
            string diaryVN = NgoEx.SystemTextFromTypeString("Shortcut_nikki", LanguageType.VN);
            string diaryFR = NgoEx.SystemTextFromTypeString("Shortcut_nikki", LanguageType.FR);
            string diaryIT = NgoEx.SystemTextFromTypeString("Shortcut_nikki", LanguageType.IT);
            string diaryGE = NgoEx.SystemTextFromTypeString("Shortcut_nikki", LanguageType.GE);
            string diarySP = NgoEx.SystemTextFromTypeString("Shortcut_nikki", LanguageType.SP);
            string diaryRU = NgoEx.SystemTextFromTypeString("Shortcut_nikki", LanguageType.RU);
            if (title == diaryJP)
                return true;
            if (title == diaryEN)
                return true;
            if (title == diaryCN)
                return true;
            if (title == diaryKO)
                return true;
            if (title == diaryTW)
                return true;
            if (title == diaryVN)
                return true;
            if (title == diaryFR)
                return true;
            if (title == diaryIT)
                return true;
            if (title == diaryGE)
                return true;
            if (title == diarySP)
                return true;
            if (title == diaryRU)
                return true;
            return false;

        }

        static bool ValidLogOneTitle(string title)
        {
            string logJP = NgoEx.SystemTextFromTypeString("Shortcut_log1", LanguageType.JP);
            string logEN = NgoEx.SystemTextFromTypeString("Shortcut_log1", LanguageType.EN);
            string logCN = NgoEx.SystemTextFromTypeString("Shortcut_log1", LanguageType.CN);
            string logKO = NgoEx.SystemTextFromTypeString("Shortcut_log1", LanguageType.KO);
            string logTW = NgoEx.SystemTextFromTypeString("Shortcut_log1", LanguageType.TW);
            string logVN = NgoEx.SystemTextFromTypeString("Shortcut_log1", LanguageType.VN);
            string logFR = NgoEx.SystemTextFromTypeString("Shortcut_log1", LanguageType.FR);
            string logIT = NgoEx.SystemTextFromTypeString("Shortcut_log1", LanguageType.IT);
            string logGE = NgoEx.SystemTextFromTypeString("Shortcut_log1", LanguageType.GE);
            string logSP = NgoEx.SystemTextFromTypeString("Shortcut_log1", LanguageType.SP);
            string logRU = NgoEx.SystemTextFromTypeString("Shortcut_log1", LanguageType.RU);
            if (title == logJP)
                return true;
            if (title == logEN)
                return true;
            if (title == logCN)
                return true;
            if (title == logKO)
                return true;
            if (title == logTW)
                return true;
            if (title == logVN)
                return true;
            if (title == logFR)
                return true;
            if (title == logIT)
                return true;
            if (title == logGE)
                return true;
            if (title == logSP)
                return true;
            if (title == logRU)
                return true;
            return false;

        }
        static bool ValidLogTwoTitle(string title)
        {
            string logJP = NgoEx.SystemTextFromTypeString("Shortcut_log2", LanguageType.JP);
            string logEN = NgoEx.SystemTextFromTypeString("Shortcut_log2", LanguageType.EN);
            string logCN = NgoEx.SystemTextFromTypeString("Shortcut_log2", LanguageType.CN);
            string logKO = NgoEx.SystemTextFromTypeString("Shortcut_log2", LanguageType.KO);
            string logTW = NgoEx.SystemTextFromTypeString("Shortcut_log2", LanguageType.TW);
            string logVN = NgoEx.SystemTextFromTypeString("Shortcut_log2", LanguageType.VN);
            string logFR = NgoEx.SystemTextFromTypeString("Shortcut_log2", LanguageType.FR);
            string logIT = NgoEx.SystemTextFromTypeString("Shortcut_log2", LanguageType.IT);
            string logGE = NgoEx.SystemTextFromTypeString("Shortcut_log2", LanguageType.GE);
            string logSP = NgoEx.SystemTextFromTypeString("Shortcut_log2", LanguageType.SP);
            string logRU = NgoEx.SystemTextFromTypeString("Shortcut_log2", LanguageType.RU);
            if (title == logJP)
                return true;
            if (title == logEN)
                return true;
            if (title == logCN)
                return true;
            if (title == logKO)
                return true;
            if (title == logTW)
                return true;
            if (title == logVN)
                return true;
            if (title == logFR)
                return true;
            if (title == logIT)
                return true;
            if (title == logGE)
                return true;
            if (title == logSP)
                return true;
            if (title == logRU)
                return true;
            return false;

        }

        static bool ValidLogThreeTitle(string title)
        {
            string logJP = NgoEx.SystemTextFromTypeString("Shortcut_log3", LanguageType.JP);
            string logEN = NgoEx.SystemTextFromTypeString("Shortcut_log3", LanguageType.EN);
            string logCN = NgoEx.SystemTextFromTypeString("Shortcut_log3", LanguageType.CN);
            string logKO = NgoEx.SystemTextFromTypeString("Shortcut_log3", LanguageType.KO);
            string logTW = NgoEx.SystemTextFromTypeString("Shortcut_log3", LanguageType.TW);
            string logVN = NgoEx.SystemTextFromTypeString("Shortcut_log3", LanguageType.VN);
            string logFR = NgoEx.SystemTextFromTypeString("Shortcut_log3", LanguageType.FR);
            string logIT = NgoEx.SystemTextFromTypeString("Shortcut_log3", LanguageType.IT);
            string logGE = NgoEx.SystemTextFromTypeString("Shortcut_log3", LanguageType.GE);
            string logSP = NgoEx.SystemTextFromTypeString("Shortcut_log3", LanguageType.SP);
            string logRU = NgoEx.SystemTextFromTypeString("Shortcut_log3", LanguageType.RU);
            if (title == logJP)
                return true;
            if (title == logEN)
                return true;
            if (title == logCN)
                return true;
            if (title == logKO)
                return true;
            if (title == logTW)
                return true;
            if (title == logVN)
                return true;
            if (title == logFR)
                return true;
            if (title == logIT)
                return true;
            if (title == logGE)
                return true;
            if (title == logSP)
                return true;
            if (title == logRU)
                return true;
            return false;

        }
        static bool ValidLogFourTitle(string title)
        {
            string logJP = NgoEx.SystemTextFromTypeString("Shortcut_log4", LanguageType.JP);
            string logEN = NgoEx.SystemTextFromTypeString("Shortcut_log4", LanguageType.EN);
            string logCN = NgoEx.SystemTextFromTypeString("Shortcut_log4", LanguageType.CN);
            string logKO = NgoEx.SystemTextFromTypeString("Shortcut_log4", LanguageType.KO);
            string logTW = NgoEx.SystemTextFromTypeString("Shortcut_log4", LanguageType.TW);
            string logVN = NgoEx.SystemTextFromTypeString("Shortcut_log4", LanguageType.VN);
            string logFR = NgoEx.SystemTextFromTypeString("Shortcut_log4", LanguageType.FR);
            string logIT = NgoEx.SystemTextFromTypeString("Shortcut_log4", LanguageType.IT);
            string logGE = NgoEx.SystemTextFromTypeString("Shortcut_log4", LanguageType.GE);
            string logSP = NgoEx.SystemTextFromTypeString("Shortcut_log4", LanguageType.SP);
            string logRU = NgoEx.SystemTextFromTypeString("Shortcut_log4", LanguageType.RU);
            if (title == logJP)
                return true;
            if (title == logEN)
                return true;
            if (title == logCN)
                return true;
            if (title == logKO)
                return true;
            if (title == logTW)
                return true;
            if (title == logVN)
                return true;
            if (title == logFR)
                return true;
            if (title == logIT)
                return true;
            if (title == logGE)
                return true;
            if (title == logSP)
                return true;
            if (title == logRU)
                return true;
            return false;

        }

        static bool ValidSecretTitle(string title)
        {
            string secretJP = NgoEx.SystemTextFromTypeString("System_secret", LanguageType.JP);
            string secretEN = NgoEx.SystemTextFromTypeString("System_secret", LanguageType.EN);
            string secretCN = NgoEx.SystemTextFromTypeString("System_secret", LanguageType.CN);
            string secretKO = NgoEx.SystemTextFromTypeString("System_secret", LanguageType.KO);
            string secretTW = NgoEx.SystemTextFromTypeString("System_secret", LanguageType.TW);
            string secretVN = NgoEx.SystemTextFromTypeString("System_secret", LanguageType.VN);
            string secretFR = NgoEx.SystemTextFromTypeString("System_secret", LanguageType.FR);
            string secretIT = NgoEx.SystemTextFromTypeString("System_secret", LanguageType.IT);
            string secretGE = NgoEx.SystemTextFromTypeString("System_secret", LanguageType.GE);
            string secretSP = NgoEx.SystemTextFromTypeString("System_secret", LanguageType.SP);
            string secretRU = NgoEx.SystemTextFromTypeString("System_secret", LanguageType.RU);
            if (title == secretJP)
                return true;
            if (title == secretEN)
                return true;
            if (title == secretCN)
                return true;
            if (title == secretKO)
                return true;
            if (title == secretTW)
                return true;
            if (title == secretVN)
                return true;
            if (title == secretFR)
                return true;
            if (title == secretIT)
                return true;
            if (title == secretGE)
                return true;
            if (title == secretSP)
                return true;
            if (title == secretRU)
                return true;
            return false;

        }
    }
}

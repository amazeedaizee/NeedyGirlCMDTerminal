using ngov3;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

namespace NeedyGirlCMDServer
{
    class TweetCommands
    {
        readonly static string[] tweetRead = { "read", "r" };
        internal static string SelectTweetCommand(string input)
        {
            IWindow window;
            string userToRead;
            var seperator = new Regex(@"\s+");
            string[] commands = seperator.Split(input, 3);
            bool isDataActive = SceneManager.GetActiveScene().name != "BiosToLoad" && SceneManager.GetActiveScene().name != "ChoozeZip";
            bool isWindowActive = SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.Poketter);
            if (!isDataActive || (!SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable && !isWindowActive))
            {
                return ErrorMessages.CMD_SPECIFIC_BUSY;
            }
            if (commands.Length == 1)
            {
                if (!isWindowActive)
                {
                    if (!SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable)
                        return "Can't do this command now.";
                    SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.Poketter);
                }
                else SingletonMonoBehaviour<WindowManager>.Instance.GetWindowFromApp(AppType.Poketter).Touched();

                return "";
            }
            if (commands.Length == 2 && isWindowActive)
            {
                window = SingletonMonoBehaviour<WindowManager>.Instance.GetWindowFromApp(AppType.Poketter);
                if (!(window._close.interactable || window._maximize.interactable || window._minimize.interactable))
                    return "Can't modify the Tweeter window now.";
                if (WindowCommands.IsWindowScroll(window, commands[1]))
                {
                    return "";
                }
                if (!WindowCommands.ChangeWindowState(window, commands[1]))
                {
                    return "Invalid command for the Social Media window.";
                }
                return "";
            }
            if (commands.Length < 3)
            {
                return ErrorMessages.CMD_WRONG_ARGS;
            }
            if (CommandManager.IsInputMatchCmd(commands[1], tweetRead))
            {
                userToRead = commands[2];
                return ReadTweet(userToRead);
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
            var tweetHistory = SingletonMonoBehaviour<PoketterManager>.Instance.history;
            return $"Number of total Tweets: {tweetHistory.Count}";
        }


        internal static string ReadTweet(string input)
        {

            WindowManager windowManager;
            windowManager = SingletonMonoBehaviour<WindowManager>.Instance;
            if (!SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.Poketter))
            {
                if (!SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable)
                    return "Can't open Tweeter from the command line now.";
                else windowManager.NewWindow(AppType.Poketter);
            }
            if (input == "last")
            {
                return ReadTweet(true);
            }
            else if (input == "first")
            {
                return ReadTweet(false);
            }
            else if (input == "first ame")
            {
                return ReadTweet(false, false);
            }
            else if (input == "last ame")
            {
                return ReadTweet(true, false);
            }
            else if (input == "first kangel")
            {
                return ReadTweet(false, true);
            }
            else if (input == "last kangel")
            {
                return ReadTweet(true, true);
            }
            else
            {
                if (!int.TryParse(input, out var num))
                    return "Message index must be a number.";
                return ReadTweet(num);
            }
        }
        internal static string ReadTweet(bool isLastTweet, bool? isAngel = null)
        {
            string user = "";
            string message = "";
            string replies = "";
            PoketterManager poketterManager;
            List<TweetData> tweetHistory;
            poketterManager = SingletonMonoBehaviour<PoketterManager>.Instance;
            tweetHistory = poketterManager.history.FindAll(t => true);
            if (tweetHistory.Count == 0)
            {
                return "";
            }
            if (isLastTweet)
            {
                tweetHistory.Reverse();
            }
            for (int i = 0; i < tweetHistory.Count; i++)
            {
                if (isAngel != null && isAngel.Value != tweetHistory[i].IsOmote)
                    continue;
                user = tweetHistory[i].IsOmote ? "KAngel:" : "Ame:";
                message = ConvertDataToTweet(tweetHistory[i]);
                if (tweetHistory[i].IsOmote && tweetHistory[i].kusoReps.Count > 0)
                {
                    replies = "\n\nReplies: \n-" + string.Join("\n- ", ConvertDataToTweetReply(tweetHistory[i].kusoReps));
                }
                break;
            }
            return $"{user}\n{message}{replies}";
        }

        internal static string ReadTweet(int input)
        {
            string user;
            string message;
            string replies = "";
            PoketterManager poketterManager;
            ReadOnlyCollection<TweetData> tweetHistory;
            poketterManager = SingletonMonoBehaviour<PoketterManager>.Instance;
            tweetHistory = poketterManager.history.AsReadOnly();
            if (tweetHistory.Count == 0)
            {
                return "Tweet History is empty.";
            }
            else if (input >= tweetHistory.Count || input < 0)
            {
                return "Tweet index value is out of bounds";
            }
            user = tweetHistory[input].IsOmote ? "KAngel:" : "Ame:";
            message = ConvertDataToTweet(tweetHistory[input]);
            if (tweetHistory[input].IsOmote && tweetHistory[input].kusoReps.Count > 0)
            {
                replies = "\n\nReplies: \n-" + string.Join("\n- ", ConvertDataToTweetReply(tweetHistory[input].kusoReps));
            }
            return $"{user}\n{message}{replies}";
        }

        private static string ConvertDataToTweet(TweetData tweetData)
        {
            var tweet = new TweetDrawable(tweetData);
            var lang = SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value;
            switch (lang)
            {
                case LanguageType.JP:
                    return tweet.BodyJp;
                case LanguageType.CN:
                    return tweet.BodyCn;
                case LanguageType.KO:
                    return tweet.BodyKo;
                case LanguageType.TW:
                    return tweet.BodyTw;
                case LanguageType.VN:
                    return tweet.BodyVn;
                case LanguageType.FR:
                    return tweet.BodyFr;
                case LanguageType.IT:
                    return tweet.BodyIt;
                case LanguageType.GE:
                    return tweet.BodyGe;
                case LanguageType.SP:
                    return tweet.BodySp;
                case LanguageType.RU:
                    return tweet.BodyRu;
                default:
                    return tweet.BodyEn;
            }
        }

        private static List<string> ConvertDataToTweetReply(List<KusoRepType> replyData)
        {
            KusoRepDrawable reply;
            List<string> replyList = new();
            var lang = SingletonMonoBehaviour<Settings>.Instance.CurrentLanguage.Value;

            foreach (var replyType in replyData)
            {
                reply = new KusoRepDrawable(replyType);
                switch (lang)
                {
                    case LanguageType.JP:
                        replyList.Add(reply.BodyJp);
                        break;
                    case LanguageType.CN:
                        replyList.Add(reply.BodyCn);
                        break;
                    case LanguageType.KO:
                        replyList.Add(reply.BodyKo);
                        break;
                    case LanguageType.TW:
                        replyList.Add(reply.BodyTw);
                        break;
                    case LanguageType.VN:
                        replyList.Add(reply.BodyVn);
                        break;
                    case LanguageType.FR:
                        replyList.Add(reply.BodyFr);
                        break;
                    case LanguageType.IT:
                        replyList.Add(reply.BodyIt);
                        break;
                    case LanguageType.GE:
                        replyList.Add(reply.BodyGe);
                        break;
                    case LanguageType.SP:
                        replyList.Add(reply.BodySp);
                        break;
                    case LanguageType.RU:
                        replyList.Add(reply.BodyRu);
                        break;
                    default:
                        replyList.Add(reply.BodyEn);
                        break;
                }

            }

            return replyList;
        }
    }
}

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
        readonly static string[] tweetFollow = { "follow", "f" };
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
                return MsgManager.SendMessage(ServerMessage.CMD_SPECIFIC_BUSY);
            }
            if (commands.Length == 1)
            {
                if (!isWindowActive)
                {
                    if (!SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable)
                        return MsgManager.SendMessage(ServerMessage.CMD_SPECIFIC_BUSY);
                    SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.Poketter);
                }
                else SingletonMonoBehaviour<WindowManager>.Instance.GetWindowFromApp(AppType.Poketter).Touched();

                return "";
            }
            if (commands.Length == 2 && isWindowActive)
            {
                window = SingletonMonoBehaviour<WindowManager>.Instance.GetWindowFromApp(AppType.Poketter);
                if (!(window._close.interactable || window._maximize.interactable || window._minimize.interactable))
                    return MsgManager.SendMessage(ServerMessage.TWEET_NO_WIN_MODIFY);
                if (WindowCommands.IsWindowScroll(window, commands[1]))
                {
                    return "";
                }
                if (!WindowCommands.ChangeWindowState(window, commands[1]))
                {
                    return MsgManager.SendMessage(ServerMessage.TWEET_WIN_INVALID_CMD);
                }
                return "";
            }
            if (commands.Length < 3)
            {
                return MsgManager.SendMessage(ServerMessage.CMD_WRONG_ARGS);
            }
            if (CommandManager.IsInputMatchCmd(commands[1], tweetRead))
            {
                userToRead = commands[2];
                return ReadTweet(userToRead);
            }
            if (CommandManager.IsInputMatchCmd(commands[1], tweetFollow))
            {
                if (SingletonMonoBehaviour<EventManager>.Instance.nowEnding == NGO.EndingType.Ending_Happy && SingletonMonoBehaviour<WindowManager>.Instance.isAppActive(AppType.Uratter))
                    return FollowUser(commands[2]);
                return MsgManager.SendMessage(ServerMessage.CMD_SPECIFIC_BUSY);
            }
            if (commands[1] == "history")
            {
                var count = seperator.Split(commands[2], 2);
                if (count[0] == "count")
                    return HistoryCount();
            }
            return MsgManager.SendMessage(ServerMessage.INVALID_CMD);
        }

        internal static string HistoryCount()
        {
            var tweetHistory = SingletonMonoBehaviour<PoketterManager>.Instance.history;
            return MsgManager.SendMessage(ServerMessage.TWEET_HISTORY_COUNT, tweetHistory.Count);
        }


        internal static string FollowUser(string command)
        {
            bool canSend = false;
            var lang = SingletonMonoBehaviour<Settings>.instance.CurrentLanguage.Value;
            var ura = SingletonMonoBehaviour<EndingHappyUraUra>.Instance;
            if (command == "raincandy_U" || command == "x_angelkawaii_x")
            {
                return MsgManager.SendMessage(ServerMessage.TWEET_MISC_FOLLOW);
            }
            string ameJP = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower1", LanguageType.JP);
            string ameEN = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower1", LanguageType.EN);
            string ameCN = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower1", LanguageType.CN);
            string ameKO = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower1", LanguageType.KO);
            string ameTW = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower1", LanguageType.TW);
            string ameVN = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower1", LanguageType.VN);
            string ameFR = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower1", LanguageType.FR);
            string ameIT = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower1", LanguageType.IT);
            string ameGE = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower1", LanguageType.GE);
            string ameSP = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower1", LanguageType.SP);
            string ameRU = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower1", LanguageType.RU);

            if (command == ameJP)
                canSend = true;
            if (command == ameEN)
                canSend = true;
            if (command == ameCN)
                canSend = true;
            if (command == ameKO)
                canSend = true;
            if (command == ameTW)
                canSend = true;
            if (command == ameVN)
                canSend = true;
            if (command == ameFR)
                canSend = true;
            if (command == ameIT)
                canSend = true;
            if (command == ameGE)
                canSend = true;
            if (command == ameSP)
                canSend = true;
            if (command == ameRU)
                canSend = true;
            if (canSend)
            {

                if (!ura._blocked.blocksRaycasts)
                    ura._followRequest.onClick.Invoke();
                return MsgManager.SendMessage(ServerMessage.TWEET_SEND_FOLLOW, NgoEx.SystemTextFromTypeString("Ending_Happy_Follower1", lang));
            }


            string twoJP = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower2", LanguageType.JP);
            string twoEN = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower2", LanguageType.EN);
            string twoCN = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower2", LanguageType.CN);
            string twoKO = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower2", LanguageType.KO);
            string twoTW = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower2", LanguageType.TW);
            string twoVN = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower2", LanguageType.VN);
            string twoFR = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower2", LanguageType.FR);
            string twoIT = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower2", LanguageType.IT);
            string twoGE = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower2", LanguageType.GE);
            string twoSP = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower2", LanguageType.SP);
            string twoRU = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower2", LanguageType.RU);
            string threeJP = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower3", LanguageType.JP);
            string threeEN = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower3", LanguageType.EN);
            string threeCN = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower3", LanguageType.CN);
            string threeKO = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower3", LanguageType.KO);
            string threeTW = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower3", LanguageType.TW);
            string threeVN = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower3", LanguageType.VN);
            string threeFR = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower3", LanguageType.FR);
            string threeIT = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower3", LanguageType.IT);
            string threeGE = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower3", LanguageType.GE);
            string threeSP = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower3", LanguageType.SP);
            string threeRU = NgoEx.SystemTextFromTypeString("Ending_Happy_Follower3", LanguageType.RU);
            if (command == twoJP)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == twoEN)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == twoCN)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == twoKO)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == twoTW)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == twoVN)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == twoFR)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == twoIT)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == twoGE)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == twoSP)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == twoRU)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == threeJP)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == threeEN)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == threeCN)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == threeKO)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == threeTW)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == threeVN)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == threeFR)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == threeIT)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == threeGE)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == threeSP)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);
            if (command == threeRU)
                return MsgManager.SendMessage(ServerMessage.TWEET_NO_FOLLOW);

            return MsgManager.SendMessage(ServerMessage.TWEET_NULL_FOLLOW);
        }

        internal static string ReadTweet(string input)
        {

            WindowManager windowManager;
            windowManager = SingletonMonoBehaviour<WindowManager>.Instance;
            if (!SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.Poketter))
            {
                if (!SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable)
                    return MsgManager.SendMessage(ServerMessage.TWEET_BUSY);
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
                    return MsgManager.SendMessage(ServerMessage.TWEET_MSG_NAN);
                return ReadTweet(num - 1);
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
                user = tweetHistory[i].IsOmote ? "x_angelkawaii_x:" : "raincandy_U:";
                message = ConvertDataToTweet(tweetHistory[i]);
                if (tweetHistory[i].IsOmote && tweetHistory[i].kusoReps.Count > 0)
                {
                    replies = MsgManager.SendMessage(ServerMessage.TWEET_REPLIES, string.Join("\n- ", ConvertDataToTweetReply(tweetHistory[i].kusoReps)));
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
                return MsgManager.SendMessage(ServerMessage.TWEET_HISTORY_EMPTY);
            }
            else if (input >= tweetHistory.Count || input < 0)
            {
                return MsgManager.SendMessage(ServerMessage.TWEET_MSG_OUTRANGE);
            }
            user = tweetHistory[input].IsOmote ? "x_angelkawaii_x:" : "raincandy_U:";
            message = ConvertDataToTweet(tweetHistory[input]);
            if (tweetHistory[input].IsOmote && tweetHistory[input].kusoReps.Count > 0)
            {
                replies = MsgManager.SendMessage(ServerMessage.TWEET_REPLIES, string.Join("\n- ", ConvertDataToTweetReply(tweetHistory[input].kusoReps)));
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

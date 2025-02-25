using ngov3;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

namespace NeedyGirlCMDServer
{
    internal class StreamCommands
    {
        readonly static string[] streamSkip = { "skip", "x" };
        readonly static string[] streamSpeed = { "speed", "s" };
        readonly static string[] streamComment = { "comment", "c" };

        readonly static string[] streamGenAdd = { "add", "a" };
        readonly static string[] streamGenEdit = { "edit", "e" };
        readonly static string[] streamGenDelete = { "delete", "d" };
        readonly static string[] streamGenStart = { "start", "play", "p" };
        readonly static string[] streamGenReplay = { "replay" };
        readonly static string[] streamGenReset = { "reset" };
        readonly static string[] streamGenGreen = { "green", "g" };

        readonly static string[] commentSelect = { "select", "s" };
        readonly static string[] commentRead = { "read", "r" };
        readonly static string[] commentSuper = { "super", "s" };
        internal static string ChooseStreamCommand(string input)
        {
            Live live;
            var seperator = new Regex(@"\s+");
            string[] commands = seperator.Split(input, 3);
            bool isBootActive = SceneManager.GetActiveScene().name == "BiosToLoad";
            bool isDataActive = SceneManager.GetActiveScene().name != "BiosToLoad" && SceneManager.GetActiveScene().name != "ChoozeZip";


            if (isBootActive)
            {
                if (!SingletonMonoBehaviour<Boot>.Instance.Login.interactable)
                    return MsgManager.SendMessage(ServerMessage.CMD_SPECIFIC_BUSY);
                if (commands.Length == 1) return OpenStreamGen();
                if (CommandManager.IsInputMatchCmd(commands[1], streamGenAdd))
                {
                    var anim = "";
                    var text = "";
                    var others = seperator.Split(commands[2].Trim(), 2);
                    if (commands.Length > 2 && others[0] != "!") anim = others[0];
                    if (others.Length == 2) text = others[1];
                    return AddKey(anim, text);
                }
                if (CommandManager.IsInputMatchCmd(commands[1], streamGenEdit))
                {
                    var anim = "";
                    var text = "";
                    if (commands.Length < 3)
                        return MsgManager.SendMessage(ServerMessage.CMD_MISSING_ARGS);
                    var others = seperator.Split(commands[2].Trim(), 3);
                    if (!int.TryParse(others[0], out int idx))
                    {
                        return MsgManager.SendMessage(ServerMessage.STREAM_GEN_IDX_NAN);
                    }
                    if (commands.Length > 3 && others[1] != "!") anim = others[1];
                    if (commands.Length == 5) text = others[2];
                    return EditKey(idx - 1, anim, text);
                }
                if (CommandManager.IsInputMatchCmd(commands[1], streamGenDelete))
                {
                    if (commands.Length < 3) return MsgManager.SendMessage(ServerMessage.CMD_MISSING_ARGS);
                    var others = seperator.Split(commands[2].Trim(), 2);
                    if (!int.TryParse(others[0], out int idx))
                    {
                        return MsgManager.SendMessage(ServerMessage.STREAM_GEN_IDX_NAN);
                    }
                    return RemoveKey(idx - 1);
                }
                if (CommandManager.IsInputMatchCmd(commands[1], streamSpeed))
                {
                    if (commands.Length < 3) return MsgManager.SendMessage(ServerMessage.CMD_MISSING_ARGS);
                    var others = seperator.Split(commands[2].Trim(), 2);
                    if (!int.TryParse(others[0], out int speed))
                    {
                        return MsgManager.SendMessage(ServerMessage.STREAM_SPEED_NAN);
                    }
                    return ChangeGenSpeed(speed);
                }
                if (CommandManager.IsInputMatchCmd(commands[1], streamGenStart)) return StartGenStream();
                if (CommandManager.IsInputMatchCmd(commands[1], streamGenReplay)) return RestartGenStream();
                if (CommandManager.IsInputMatchCmd(commands[1], streamGenReset)) return ResetGenStream();
                if (CommandManager.IsInputMatchCmd(commands[1], streamGenGreen)) return GreenGenStream();
            }
            else
            {
                if (!isDataActive)
                {
                    return MsgManager.SendMessage(ServerMessage.CMD_SPECIFIC_BUSY);
                }
                live = SingletonMonoBehaviour<Live>.Instance;
                if (!live) return MsgManager.SendMessage(ServerMessage.STREAM_INACTIVE);
                if (CommandManager.IsInputMatchCmd(commands[1], streamSkip))
                {
                    return SkipStream(live);
                }
                if (CommandManager.IsInputMatchCmd(commands[1], streamSpeed))
                {
                    if (commands.Length < 3) return MsgManager.SendMessage(ServerMessage.CMD_MISSING_ARGS);
                    var others = seperator.Split(commands[2].Trim(), 2);
                    if (!int.TryParse(others[0], out int speed))
                    {
                        return MsgManager.SendMessage(ServerMessage.STREAM_SPEED_NAN);
                    }
                    return ChangeSpeed(live, speed);
                }
                if (CommandManager.IsInputMatchCmd(commands[1], streamComment))
                {
                    var others = seperator.Split(commands[2].Trim(), 3);
                    if (others.Length < 2)
                    {
                        return MsgManager.SendMessage(ServerMessage.STREAM_COMM_MISSING_ARGS);
                    }
                    if (CommandManager.IsInputMatchCmd(others[0], commentSelect))
                    {
                        if (CommandManager.IsInputMatchCmd(others[1], commentSuper))
                        {
                            return SelectSuperChat(live);
                        }
                        if (!int.TryParse(others[1], out int selectedChat))
                        {
                            return MsgManager.SendMessage(ServerMessage.STREAM_COMM_IDX_NAN);
                        }
                        return SelectComment(live, selectedChat - 1);
                    }
                    if (CommandManager.IsInputMatchCmd(others[0], commentRead))
                    {
                        if (CommandManager.IsInputMatchCmd(others[1], commentSuper))
                        {
                            return ReadSuperComment(live);
                        }
                        if (!int.TryParse(others[1], out int readChat))
                        {
                            return MsgManager.SendMessage(ServerMessage.STREAM_COMM_IDX_NAN);
                        }
                        return ReadComment(live, readChat - 1);
                    }
                }
            }

            return MsgManager.SendMessage(ServerMessage.INVALID_CMD);
        }
        internal static string SkipStream(Live live)
        {
            if (!live._HaisinSkip.gameObject.activeInHierarchy)
            {
                return MsgManager.SendMessage(ServerMessage.STREAM_UNSKIPPABLE);
            }
            live.NowPlaying.SkipScenario();
            return "";
        }
        internal static string ChangeSpeed(Live live, int input)
        {
            if (!(input > 0 && input < 4))
                return MsgManager.SendMessage(ServerMessage.STREAM_SPEED_OUTRANGE);
            if (!live._HaisinSpeed.gameObject.activeInHierarchy)
            {
                return MsgManager.SendMessage(ServerMessage.STREAM_SPEED_LOCK);
            }
            input--;
            live.setSpeed(input);
            return "";
        }
        internal static string SelectComment(Live live, int input)
        {
            Playing playing;
            if (!live.isActiveReaction())
            {
                return MsgManager.SendMessage(ServerMessage.STREAM_READ_ONLY);
            }
            if (input >= live._selectableComments.Count)
            {
                return MsgManager.SendMessage(ServerMessage.STREAM_COMM_OVERRANGE);
            }
            if (input < 0)
            {
                return MsgManager.SendMessage(ServerMessage.STREAM_COMM_UNDERRANGE);
            }
            if (!live._selectableComments[input].isHiroizumi)
            {
                playing = live._selectableComments[input].playing;
                if (playing.henji != "")
                {
                    live._selectableComments[input].hirou(playing);
                }
                else if (SingletonMonoBehaviour<EventManager>.Instance.nowEnding == NGO.EndingType.Ending_KowaiInternet)
                {
                    live._selectableComments[input].highlighted();
                }
                else
                {
                    live._selectableComments[input].sakujo(playing);
                }
            }
            return "";
        }

        internal static string ReadComment(Live live, int input)
        {
            string message;
            if (input >= live._selectableComments.Count)
            {
                return MsgManager.SendMessage(ServerMessage.STREAM_COMM_OVERRANGE);
            }
            if (input < 0)
            {
                return MsgManager.SendMessage(ServerMessage.STREAM_COMM_UNDERRANGE);
            }
            if (live._selectableComments[input].isDeleted)
            {
                return MsgManager.SendMessage(ServerMessage.STREAM_COMM_DELETED);
            }
            message = live._selectableComments[input].honbun;
            return MsgManager.SendMessage(ServerMessage.STREAM_COMM_READ, message);
        }

        internal static string ReadSuperComment(Live live)
        {
            string message;
            Playing playing;
            LiveComment comment = null;
            for (int i = live._selectableComments.Count - 1; i >= 0; i--)
            {
                playing = live._selectableComments[i].playing;
                if (playing.henji != "")
                {
                    comment = live._selectableComments[i];
                    break;
                }
            }
            if (comment == null)
            {
                return MsgManager.SendMessage(ServerMessage.STREAM_NO_SUPERS);
            }
            message = comment.honbun;
            return MsgManager.SendMessage(ServerMessage.STREAM_COMM_READ, message);
        }

        internal static string SelectSuperChat(Live live)
        {
            Playing playing;
            LiveComment comment = null;
            if (!live.isActiveReaction())
            {
                return MsgManager.SendMessage(ServerMessage.STREAM_READ_ONLY);
            }
            for (int i = live._selectableComments.Count - 1; i >= 0; i--)
            {
                playing = live._selectableComments[i].playing;
                if (playing.henji != "" && !live._selectableComments[i].isHiroizumi)
                {
                    comment = live._selectableComments[i];
                    break;
                }
            }
            if (comment == null)
            {
                return MsgManager.SendMessage(ServerMessage.STREAM_NO_SUPERS);
            }
            playing = comment.playing;
            comment.hirou(playing);
            return "";
        }

        internal static string OpenStreamGen()
        {
            bool isWindowActive = SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.ManualHaishin);
            if (!isWindowActive)
                SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.ManualHaishin);
            return "";
        }

        internal static string AddKey(string anim, string text)
        {
            bool isWindowActive = SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.ManualHaishin);
            if (!isWindowActive) return MsgManager.SendMessage(ServerMessage.STREAM_GEN_INACTIVE);

            var win = SingletonMonoBehaviour<Live_gen>.Instance;
            int len = win.textInputs.Count;
            foreach (var t in win.textInputs)
            {
                t.SetSelected(false);
            }
            win.textInputs[len - 1].CreateChildItem();
            var select = win.textInputs.FirstOrDefault(t => t.IsSelected);
            select.AnimationKey.Value = anim;
            select.Input.text = text;
            return "";
        }

        internal static string EditKey(int idx, string anim, string text)
        {
            bool isWindowActive = SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.ManualHaishin);
            if (!isWindowActive) return MsgManager.SendMessage(ServerMessage.STREAM_GEN_INACTIVE);

            var win = SingletonMonoBehaviour<Live_gen>.Instance;
            int len = win.textInputs.Count;
            if (idx < 0 || idx >= len)
            {

                return MsgManager.SendMessage(ServerMessage.STREAM_GEN_IDX_OUTRANGE);
            }
            var select = win.textInputs[idx];
            foreach (var t in win.textInputs)
            {
                t.SetSelected(false);
            }
            select.SetSelected(true);
            select.AnimationKey.Value = anim;
            select.Input.text = text;
            return "";
        }

        internal static string RemoveKey(int idx)
        {
            bool isWindowActive = SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.ManualHaishin);
            if (!isWindowActive) return MsgManager.SendMessage(ServerMessage.STREAM_GEN_INACTIVE);

            var win = SingletonMonoBehaviour<Live_gen>.Instance;
            int len = win.textInputs.Count;
            if (idx < 0 || idx >= len)
            {
                return MsgManager.SendMessage(ServerMessage.STREAM_GEN_IDX_OUTRANGE);
            }
            var select = win.textInputs[idx];
            foreach (var t in win.textInputs)
            {
                t.SetSelected(false);
            }
            select.DeleatItem();
            return "";
        }

        internal static string ChangeGenSpeed(int idx)
        {
            bool isWindowActive = SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.ManualHaishin);
            if (!isWindowActive) return MsgManager.SendMessage(ServerMessage.STREAM_GEN_INACTIVE);

            var win = SingletonMonoBehaviour<Live_gen>.Instance;
            if (idx < 1 || idx > 3) return MsgManager.SendMessage(ServerMessage.STREAM_GEN_IDX_OUTRANGE);
            win.setSpeed(idx - 1);
            return "";
        }

        internal static string StartGenStream()
        {
            bool isWindowActive = SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.ManualHaishin);
            if (!isWindowActive) return MsgManager.SendMessage(ServerMessage.STREAM_GEN_INACTIVE);

            var win = SingletonMonoBehaviour<Live_gen>.Instance;
            win.readline();
            return "";
        }
        internal static string RestartGenStream()
        {
            bool isWindowActive = SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.ManualHaishin);
            if (!isWindowActive) return MsgManager.SendMessage(ServerMessage.STREAM_GEN_INACTIVE);

            var win = SingletonMonoBehaviour<Live_gen>.Instance;
            win.rewind();
            return "";
        }

        internal static string ResetGenStream()
        {
            bool isWindowActive = SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.ManualHaishin);
            if (!isWindowActive) return MsgManager.SendMessage(ServerMessage.STREAM_GEN_INACTIVE);

            var win = SingletonMonoBehaviour<Live_gen>.Instance;
            win.clear();
            return "";
        }

        internal static string GreenGenStream()
        {
            bool isWindowActive = SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.ManualHaishin);
            if (!isWindowActive) return MsgManager.SendMessage(ServerMessage.STREAM_GEN_INACTIVE);

            var win = SingletonMonoBehaviour<Live_gen>.Instance;
            win.toggleGB();
            return "";
        }
    }
}

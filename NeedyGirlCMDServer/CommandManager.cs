using Cysharp.Threading.Tasks;
using ngov3;
using ngov3.Effect;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

namespace NeedyGirlCMDServer
{
    internal class CommandManager
    {
        readonly static string[] helpCommand = { "help" };
        readonly static string[] windowCommand = { "w", "window" };
        readonly static string[] cautionCommand = { "caution" };
        readonly static string[] loadCommand = { "load" };
        readonly static string[] reloadCommand = { "reload" };
        readonly static string[] optionsCommand = { "options" };
        readonly static string[] resetCommand = { "reset", "restart" };
        readonly static string[] shutdownCommand = { "shutdown" };
        readonly static string[] debugCommand = { "debug" };
        readonly static string[] endingCommand = { "end", "ending" };
        readonly static string[] loginCommand = { "login" };
        readonly static string[] cutCommand = { "cut" };
        internal readonly static string[] webcamCommand = { "ame", "webcam" };
        readonly static string[] streamCommand = { "s", "stream" };
        readonly static string[] jineCommand = { "j", "jine" };
        readonly static string[] notifCommand = { "n", "notif", "notification" };
        readonly static string[] tweetCommand = { "t", "tweeter", "p", "poketter" };
        internal readonly static string[] actionCommand = { "a", "action" };
        readonly static string[] readCommand = { "read" };
        readonly static string[] openCommand = { "open" };
        readonly static string[] unzipCommand = { "unzip" };
        readonly static string[] myPicCommand = { "pic", "picture" };
        readonly static string[] infoCommand = { "info", "i" };

        internal static StreamReader streamReader;
        internal static StreamWriter streamWriter;

        internal static NetworkStream ns;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static async UniTask StartReceiveCommand()
        {
            Initializer.logger.LogInfo("Connecting to terminal...");
            ns = ConnectionManager.client.GetStream();
            streamReader = new StreamReader(ns);
            streamWriter = new StreamWriter(ns);
            streamWriter.AutoFlush = true;
            Initializer.logger.LogInfo("Successfully connected to the terminal!");
            await UniTask.Delay(500);
            while (ConnectionManager.client.Connected)
            {
                try
                {
                    await ReceiveCommand();
                    await UniTask.Delay(10);
                }
                catch { }
            }
            Initializer.logger.LogInfo("Disconnected from the terminal.");
            ConnectionManager.client.Close();
            ConnectionManager.client.Dispose();
            ConnectionManager.pipe.Stop();
            ConnectionManager.WaitForConnection().Forget();
        }

        internal static async UniTask ReceiveCommand()
        {
            string[] commands;
            string message = "";
            var seperator = new Regex(@"\s+");
            int size = 8192;
            var buf = new byte[size];
            //var sb = new StringBuilder();
            string input = "";
            await ns.ReadAsync(buf, 0, size, ConnectionManager.cts.Token);
            if (ConnectionManager.cts.IsCancellationRequested)
                return;
            var arr = Encoding.UTF8.GetString(buf);
            foreach (var c in arr)
            {

                if (c == '\0')
                {
                    break;
                }
                input += c;

            }
            input = input.Trim();
            Initializer.logger.LogInfo($"Receiving command: {input}");

            if (string.IsNullOrWhiteSpace(input))
            {
                await MouseClick.FakeMouseClick();
            }
            else if (SceneManager.GetActiveScene().name.Contains("Window") &&
                SingletonMonoBehaviour<EventManager>.Instance.nowEnding == NGO.EndingType.Ending_Meta &&
                SingletonMonoBehaviour<EndingManager>.Instance == null &&
                !SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.Broadcast))
            {
                message = MetaCommands.RespondToAngel(input);
            }
            else if (SceneManager.GetActiveScene().name == "WindowUITestScene" &&
    SingletonMonoBehaviour<EventManager>.Instance.nowEnding == NGO.EndingType.Ending_Completed &&
    !SingletonMonoBehaviour<WebCamManager>.Instance.hidegirl.Value)
            {
                message = "...";
            }
            else if (IsInputMatchCmd(input, cautionCommand, true))
            {
                message = BootCommands.CautionCommand(input);
            }
            else if (IsInputMatchCmd(input, optionsCommand, true))
            {
                message = OptionsCommands.SetOptions(input);
            }
            else if (IsInputMatchCmd(input, helpCommand, true))
            {
                message = "?>";
            }
            else if (SceneManager.GetActiveScene().name.Contains("Window") && SingletonMonoBehaviour<DayPassing2D>.Instance.playingAnimation)
            {
                message = MsgManager.SendMessage(ServerMessage.CMD_BUSY);
            }
            else if (IsInputMatchCmd(input, windowCommand, true))
            {
                commands = seperator.Split(input, 2);
                if (commands.Length == 1)
                {
                    message = MsgManager.SendMessage(ServerMessage.CMD_WRONG_ARGS);
                }
                else message = WindowCommands.SelectWindowCommand(commands[1]);
            }
            else if (IsInputMatchCmd(input, loadCommand, true))
            {
                if (SceneManager.GetActiveScene().name == "BiosToLoad")
                    message = await BootCommands.LoginScreenCommand(input);
                else if (SceneManager.GetActiveScene().name != "ChooseZip")
                    message = LoadCommands.LoadSave(input);
            }
            else if (IsInputMatchCmd(input, reloadCommand, true))
            {
                LoadCommands.ReloadSave();
            }
            else if (IsInputMatchCmd(input, resetCommand, true))
            {
                message = GameCommands.RestartGame(input);
            }
            else if (IsInputMatchCmd(input, shutdownCommand, true))
            {
                message = GameCommands.ShutDownGame(input);
            }
            else if (IsInputMatchCmd(input, debugCommand, true))
            {
                message = await DebugCommands.StartDebugCommand(input);
            }
            else if (IsInputMatchCmd(input, unzipCommand, true))
            {
                message = ZipCommands.OpenLockedZip(input);
            }
            else if (IsInputMatchCmd(input, infoCommand))
            {
                message = InfoCommands.ShowInfo();
            }
            else if (IsInputMatchCmd(input, endingCommand, true))
            {
                commands = seperator.Split(input, 3);
                if (commands.Length == 1)
                    message = NotifCommands.ClickToEnding();
                else if (commands.Length > 1 && IsInputMatchCmd(commands[1], infoCommand))
                    message = InfoCommands.ShowEndingInfo();
            }
            else if (IsInputMatchCmd(input, loginCommand, true))
            {
                message = LoginCommands.StartLogin(input);
            }
            else if (IsInputMatchCmd(input, cutCommand))
            {
                message = WristCommands.StartDestruct();
            }
            else if (IsInputMatchCmd(input, webcamCommand, true))
            {
                message = await WebcamCommands.ControlWebcam(input);
            }
            else if (IsInputMatchCmd(input, streamCommand, true))
            {
                message = StreamCommands.ChooseStreamCommand(input);
            }
            else if (IsInputMatchCmd(input, jineCommand, true))
            {
                message = await JineCommands.SelectJineCommand(input);
            }
            else if (IsInputMatchCmd(input, notifCommand))
            {
                message = NotifCommands.ClickNotif();
            }
            else if (IsInputMatchCmd(input, tweetCommand, true))
            {
                message = TweetCommands.SelectTweetCommand(input);
            }
            else if (IsInputMatchCmd(input, actionCommand, true))
            {
                message = ActionCommands.StartAction(input);
            }
            else if (IsInputMatchCmd(input, myPicCommand, true))
            {
                message = MyPicturesCommands.StartPicCommand(input);
            }
            else if (IsInputMatchCmd(input, readCommand, true))
            {
                commands = seperator.Split(input, 2);
                if (commands.Length == 1)
                {
                    message = "This command requires at least two arguments.";
                }
                else message = TextReaderCommands.ReadTextDoc(input);
            }
            else if (IsInputMatchCmd(input, openCommand, true))
            {
                commands = seperator.Split(input, 2);
                if (commands.Length == 1)
                {
                    message = "This command requires at least two arguments.";
                }
                else message = TextReaderCommands.OpenTextDoc(input);
            }
            else
            {
                message = "Invalid command.";
            }
            if (string.IsNullOrWhiteSpace(message))
                message = ">";
            SendMessage(message);
        }

        internal static bool IsInputMatchCmd(string input, string[] commands, bool hasMoreArgs = false)
        {
            for (int i = 0; i < commands.Length; i++)
            {
                if (input == commands[i])
                    return true;
                if (hasMoreArgs && input.StartsWith($"{commands[i]} "))
                    return true;
            }
            return false;
        }
        internal static void SendMessage(string message)
        {
            if (!ConnectionManager.client.Connected)
                return;
            streamReader.DiscardBufferedData();
            streamWriter.WriteLine(message);
            Initializer.logger.LogMessage("Pinging to the terminal: " + message);
            if (message != ">" && message != "?>" && message != "!>")
                streamWriter.WriteLine(">");
            streamWriter.Flush();
            //ConnectionManager.pipe.WaitForPipeDrain();
        }
    }
}

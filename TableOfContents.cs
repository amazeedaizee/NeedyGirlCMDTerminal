using System.IO;
using System.Reflection;

namespace NeedyGirlCMDTerminal
{
    internal class TableOfContents
    {
        const string HELP_NOT_FOUND = "Could not find help page for this command!";
        const string resRoot = "NeedyGirlCMDTerminal.Resources.";
        readonly static string[] helpCommand = { "help" };
        readonly static string[] parentCommand = { "parent" };
        readonly static string[] commandCommand = { "command" };
        readonly static string[] videoCommand = { "video" };
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
        readonly static string[] webcamCommand = { "ame", "webcam" };
        readonly static string[] streamCommand = { "s", "stream" };
        readonly static string[] jineCommand = { "j", "jine" };
        readonly static string[] notifCommand = { "n", "notif", "notification" };
        readonly static string[] tweetCommand = { "t", "tweeter", "p", "poketter" };
        readonly static string[] actionCommand = { "a", "action" };
        readonly static string[] readCommand = { "read" };
        readonly static string[] openCommand = { "open" };
        readonly static string[] unzipCommand = { "unzip" };
        readonly static string[] myPicCommand = { "view", "pic", "picture" };
        readonly static string[] infoCommand = { "info", "i" };

        internal static string GetHelpPage(string[] commands)
        {
            var ass = Assembly.GetExecutingAssembly();
            Stream? stream = null;
            string page = HELP_NOT_FOUND;
            //Console.Write(string.Join('|', ass.GetManifestResourceNames()));
            if (commands[0] != helpCommand[0])
            {
                return "";
            }
            if (commands.Length == 1)
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_help.txt");
                // page = Resource.Help_help;
            }
            else if (IsInputMatchCmd(commands[1], commandCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_help_command.txt");
                // page = Resource.Help_help_command;
            }
            else if (IsInputMatchCmd(commands[1], actionCommand))
            {
                if (commands.Length == 3 && IsInputMatchCmd(commands[2], parentCommand))
                {
                    stream = ass.GetManifestResourceStream(resRoot + "Help_action_parent.txt");
                    //page = Resource.Help_action_parent;
                }
                else
                {
                    stream = ass.GetManifestResourceStream(resRoot + "Help_action_main.txt");
                    // page = Resource.Help_action_main;
                }
            }
            else if (IsInputMatchCmd(commands[1], cautionCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_caution.txt");
                // page = Resource.Help_caution;
            }
            else if (IsInputMatchCmd(commands[1], cutCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_cut.txt");
                // page = Resource.Help_cut;
            }
            else if (IsInputMatchCmd(commands[1], debugCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_debug.txt");
                // page = Resource.Help_debug;
            }
            else if (IsInputMatchCmd(commands[1], endingCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_ending.txt");
                // page = Resource.Help_ending;
            }
            else if (IsInputMatchCmd(commands[1], infoCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_info.txt");
                // page = Resource.Help_info;
            }
            else if (IsInputMatchCmd(commands[1], jineCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_jine.txt");
                // page = Resource.Help_jine;
            }
            else if (IsInputMatchCmd(commands[1], loadCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_load.txt");
                // page = Resource.Help_load;
            }
            else if (IsInputMatchCmd(commands[1], loginCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_login.txt");
                // page = Resource.Help_login;
            }
            else if (IsInputMatchCmd(commands[1], notifCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_notif.txt");
                // page = Resource.Help_notif;
            }
            else if (IsInputMatchCmd(commands[1], openCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_open.txt");
                //page = (Resource.Help_open);
            }
            else if (IsInputMatchCmd(commands[1], optionsCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_options.txt");
                //page = (Resource.Help_options);
            }
            else if (IsInputMatchCmd(commands[1], myPicCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_pic.txt");
                //page = Resource.Help_pic;
            }
            else if (IsInputMatchCmd(commands[1], readCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_read.txt");
                //page = (Resource.Help_read);
            }
            else if (IsInputMatchCmd(commands[1], reloadCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_reload.txt");
                //page = (Resource.Help_reload);
            }
            else if (IsInputMatchCmd(commands[1], resetCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_reset.txt");
                // page = (Resource.Help_reset);
            }
            else if (IsInputMatchCmd(commands[1], shutdownCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_shutdown.txt");
                //page = (Resource.Help_shutdown);
            }
            else if (IsInputMatchCmd(commands[1], streamCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_stream.txt");
                //page = (Resource.Help_stream);
            }
            else if (IsInputMatchCmd(commands[1], tweetCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_tweet.txt");
                // page = Resource.Help_tweet;
            }
            else if (IsInputMatchCmd(commands[1], unzipCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_unzip.txt");
                // page = Resource.Help_unzip;
            }
            else if (IsInputMatchCmd(commands[1], webcamCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_webcam.txt");
                //page = (Resource.Help_webcam);
            }
            else if (IsInputMatchCmd(commands[1], windowCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_window.txt");
                // page = (Resource.Help_window);
            }
            else if (IsInputMatchCmd(commands[1], videoCommand))
            {
                stream = ass.GetManifestResourceStream(resRoot + "Help_video.txt");
                //page = Encoding.UTF8.GetString(Resource.Help_video);
            }

            if (stream != null)
            {
                using (StreamReader streamReader = new(stream))
                {
                    page = streamReader.ReadToEnd();
                }
                stream.Dispose();
            }
            return page;
        }

        internal static bool IsInputMatchCmd(string input, string[] commands)
        {
            for (int i = 0; i < commands.Length; i++)
            {
                if (input == commands[i])
                    return true;
            }
            return false;
        }
    }
}

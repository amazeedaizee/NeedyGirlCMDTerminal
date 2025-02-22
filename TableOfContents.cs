namespace NeedyGirlCMDTerminal
{
    internal class TableOfContents
    {
        const string HELP_NOT_FOUND = "Could not find help page for this command!";

        readonly static string[] helpCommand = { "help" };
        readonly static string[] parentCommand = { "parent" };
        readonly static string[] commandCommand = { "command" };

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
            string page = HELP_NOT_FOUND;
            if (commands[0] != helpCommand[0])
            {
                return "";
            }
            if (commands.Length == 1)
            {
                page = Resource.Help_help;
            }
            else if (IsInputMatchCmd(commands[1], commandCommand))
            {
                page = Resource.Help_help_command;
            }
            else if (IsInputMatchCmd(commands[1], actionCommand))
            {
                if (commands.Length == 3 && IsInputMatchCmd(commands[2], parentCommand))
                {
                    page = Resource.Help_action_parent;
                }
                else page = Resource.Help_action_main;
            }
            else if (IsInputMatchCmd(commands[1], cautionCommand))
            {
                page = Resource.Help_caution;
            }
            else if (IsInputMatchCmd(commands[1], cutCommand))
            {
                page = Resource.Help_cut;
            }
            else if (IsInputMatchCmd(commands[1], debugCommand))
            {
                page = Resource.Help_debug;
            }
            else if (IsInputMatchCmd(commands[1], endingCommand))
            {
                page = Resource.Help_ending;
            }
            else if (IsInputMatchCmd(commands[1], infoCommand))
            {
                page = Resource.Help_info;
            }
            else if (IsInputMatchCmd(commands[1], jineCommand))
            {
                page = Resource.Help_jine;
            }
            else if (IsInputMatchCmd(commands[1], loadCommand))
            {
                page = Resource.Help_load;
            }
            else if (IsInputMatchCmd(commands[1], loginCommand))
            {
                page = Resource.Help_login;
            }
            else if (IsInputMatchCmd(commands[1], notifCommand))
            {
                page = Resource.Help_notif;
            }
            else if (IsInputMatchCmd(commands[1], openCommand))
            {
                page = (Resource.Help_open);
            }
            else if (IsInputMatchCmd(commands[1], optionsCommand))
            {
                page = (Resource.Help_options);
            }
            else if (IsInputMatchCmd(commands[1], myPicCommand))
            {
                page = Resource.Help_pic;
            }
            else if (IsInputMatchCmd(commands[1], readCommand))
            {
                page = (Resource.Help_read);
            }
            else if (IsInputMatchCmd(commands[1], reloadCommand))
            {
                page = (Resource.Help_reload);
            }
            else if (IsInputMatchCmd(commands[1], resetCommand))
            {
                page = (Resource.Help_reset);
            }
            else if (IsInputMatchCmd(commands[1], shutdownCommand))
            {
                page = (Resource.Help_shutdown);
            }
            else if (IsInputMatchCmd(commands[1], streamCommand))
            {
                page = (Resource.Help_stream);
            }
            else if (IsInputMatchCmd(commands[1], tweetCommand))
            {
                page = Resource.Help_tweet;
            }
            else if (IsInputMatchCmd(commands[1], unzipCommand))
            {
                page = Resource.Help_unzip;
            }
            else if (IsInputMatchCmd(commands[1], webcamCommand))
            {
                page = (Resource.Help_webcam);
            }
            else if (IsInputMatchCmd(commands[1], windowCommand))
            {
                page = (Resource.Help_window);
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

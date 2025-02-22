using ngov3;
using System.Text.RegularExpressions;

namespace NeedyGirlCMDServer
{
    internal class ZipCommands
    {
        internal static string OpenLockedZip(string input)
        {
            var seperator = new Regex(@"\s+");
            string[] commands = seperator.Split(input, 3);
            Settings settings = SingletonMonoBehaviour<Settings>.instance;
            if (commands.Length != 2)
            {
                return ErrorMessages.CMD_WRONG_ARGS;
            }
            if (SingletonMonoBehaviour<EndingOmake>.Instance == null)
            {
                return ErrorMessages.CMD_SPECIFIC_BUSY;
            }
            else if (!int.TryParse(commands[1], out int num))
            {
                return "Specified zip number is not a number.";
            }
            else if (num < 1 || num > 30)
            {
                return "Specified zip number is out of range. (1-30)";
            }
            else if (settings.unLockedZip.Contains(num))
            {
                return "This zip is already open!";
            }
            else
            {
                SingletonMonoBehaviour<EndingOmake>.Instance.startOpen(num);
            }
            return "";

        }
    }
}

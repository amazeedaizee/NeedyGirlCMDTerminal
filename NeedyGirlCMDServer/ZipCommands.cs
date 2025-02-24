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
                return MsgManager.SendMessage(ServerMessage.CMD_WRONG_ARGS);
            }
            if (SingletonMonoBehaviour<EndingOmake>.Instance == null)
            {
                return MsgManager.SendMessage(ServerMessage.CMD_SPECIFIC_BUSY);
            }
            else if (!int.TryParse(commands[1], out int num))
            {
                return MsgManager.SendMessage(ServerMessage.ZIP_NUM_NAN);
            }
            else if (num < 1 || num > 30)
            {
                return MsgManager.SendMessage(ServerMessage.ZIP_NUM_OUTRANGE);
            }
            else if (settings.unLockedZip.Contains(num))
            {
                return MsgManager.SendMessage(ServerMessage.ZIP_OPENED);
            }
            else
            {
                SingletonMonoBehaviour<EndingOmake>.Instance.startOpen(num);
            }
            return "";

        }
    }
}

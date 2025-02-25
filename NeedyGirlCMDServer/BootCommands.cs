using Cysharp.Threading.Tasks;
using ngov3;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

namespace NeedyGirlCMDServer
{
    internal class BootCommands
    {

        internal static string CautionCommand(string input)
        {
            Boot boot;
            var seperator = new Regex(@"\s+");
            string[] commands = seperator.Split(input, 3);
            if (SceneManager.GetActiveScene().name != "BiosToLoad")
            {
                return MsgManager.SendMessage(ServerMessage.BOOT_SCENE_INVALID);
            }
            if (commands.Length < 2)
            {
                return MsgManager.SendMessage(ServerMessage.CMD_WRONG_ARGS);
            }
            boot = SingletonMonoBehaviour<Boot>.Instance;
            if (!boot.Caution.interactable)
            {
                return MsgManager.SendMessage(ServerMessage.BOOT_CAUTION_INACTIVE);
            }
            if (CommandManager.IsInputMatchCmd(commands[1], CommandManager.cancelOption))
                boot.Cancel.onClick.Invoke();
            else if (CommandManager.IsInputMatchCmd(commands[1], CommandManager.okOption))
                boot.Ok.onClick.Invoke();
            else
            {

                return MsgManager.SendMessage(ServerMessage.INVALID_CMD);
            }
            return "";
        }

        internal static async UniTask<string> LoginScreenCommand(string input)
        {
            int user;
            Boot boot = SingletonMonoBehaviour<Boot>.Instance;
            adieuDialog adieu;
            char[] seperator = { ' ' };
            string[] commands = input.Split(seperator, 4);
            if (commands.Length < 2)
            {
                return MsgManager.SendMessage(ServerMessage.CMD_WRONG_ARGS);
            }
            if (boot.Caution.interactable)
            {
                return MsgManager.SendMessage(ServerMessage.BOOT_CAUTION_ACTIVE);
            }
            if (!int.TryParse(commands[1], out user))
            {
                return MsgManager.SendMessage(ServerMessage.LOAD_USER_NAN);
            }
            if (user == 0 && boot.Data0.gameObject.activeInHierarchy)
            {
                adieu = new();
                adieu.OnSubmit();
                return "...";
            }
            if (!(user > 0 && user < 4) && !boot.ChooseDay.interactable)
            {
                return MsgManager.SendMessage(ServerMessage.LOAD_USER_OUTRANGE);
            }
            if (!(user > 0 && user < 31) && boot.ChooseDay.interactable)
            {
                return MsgManager.SendMessage(ServerMessage.LOAD_DAY_OUTRANGE);
            }
            if (commands.Length == 2)
            {
                if (boot.ChooseUser.interactable)
                {
                    boot.WaitChooseDay(user);
                    return "";
                }
                else if (boot.ChooseDay.interactable)
                {
                    return await TryLoadDay(commands[1], SingletonMonoBehaviour<Settings>.Instance.saveNumber, boot);
                }
            }
            return await TryLoadDay(commands[2], user, boot);

        }

        static async UniTask<string> TryLoadDay(string num, int user, Boot boot)
        {
            if (!int.TryParse(num, out int day))
            {
                return MsgManager.SendMessage(ServerMessage.LOAD_DAY_NAN);
            }
            if (!(day > 0 && day < 31))
            {
                return MsgManager.SendMessage(ServerMessage.LOAD_DAY_OUTRANGE);
            }
            if (day > 1 && !SaveRelayer.IsSlotDataExists($"Data{user}_Day{day}{SaveRelayer.EXTENTION}"))
            {
                return MsgManager.SendMessage(ServerMessage.LOAD_SAVE_NOT_FOUND);
            }
            boot.ChooseUser.alpha = 0f;
            boot.ChooseUser.interactable = false;
            boot.ChooseUser.blocksRaycasts = false;
            if (day == 1)
            {
                await boot.StartGame(user);
            }
            else
            {
                await boot.Resume(user, $"Data{user}_Day{day}");
            }
            return "";
        }

    }
}

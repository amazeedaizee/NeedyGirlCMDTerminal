using Cysharp.Threading.Tasks;
using ngov3;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NeedyGirlCMDServer
{
    internal class BootCommands
    {
        readonly static string[] okOption = { "ok" };
        readonly static string[] cancelOption = { "cancel" };
        internal static string CautionCommand(string input)
        {
            Boot boot;
            var seperator = new Regex(@"\s+");
            string[] commands = seperator.Split(input, 3);
            if (SceneManager.GetActiveScene().name != "BiosToLoad")
            {
                return "Scene is not Caution/Login scene.";
            }
            if (commands.Length < 2)
            {
                return ErrorMessages.CMD_WRONG_ARGS;
            }
            boot = SingletonMonoBehaviour<Boot>.Instance;
            if (!boot.Caution.interactable)
            {
                return "Caution message is not active.";
            }
            if (CommandManager.IsInputMatchCmd(commands[1], cancelOption))
                Application.Quit();
            else if (CommandManager.IsInputMatchCmd(commands[1], okOption))
                boot.Ok.onClick.Invoke();
            else
            {

                return ErrorMessages.INVALID_CMD;
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
                return ErrorMessages.CMD_WRONG_ARGS;
            }
            if (boot.Caution.interactable)
            {
                return "Caution message is still active.";
            }
            if (!int.TryParse(commands[1], out user))
            {
                return "Submitted user is not a number.";
            }
            if (user == 0 && boot.Data0.gameObject.activeInHierarchy)
            {
                adieu = new();
                adieu._submitButton.onClick.Invoke();
                return "...";
            }
            if (!(user > 0 && user < 4) && !boot.ChooseDay.interactable)
            {
                return "Submitted user is not between 1 to 3.";
            }
            if (!(user > 0 && user < 31) && boot.ChooseDay.interactable)
            {
                return "Submitted day is not between 1 to 30.";
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
                return "Submitted day is not a number.";
            }
            if (!(day > 0 && day < 31))
            {
                return "Submitted day is not between 1 to 30.";
            }
            if (day > 1 && !SaveRelayer.IsSlotDataExists($"Data{user}_Day{day}{SaveRelayer.EXTENTION}"))
            {
                return "Submitted user/day combination does not exist.";
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

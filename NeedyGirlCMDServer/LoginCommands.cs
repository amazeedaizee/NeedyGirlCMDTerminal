using ngov3;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NeedyGirlCMDServer
{
    internal class LoginCommands
    {
        internal static string StartLogin(string input)
        {

            string password;
            Login login = null;
            var loginShortcut = GameObject.Find("LoginShortCut").GetComponent<CanvasGroup>();
            var seperator = new Regex(@"\s+");
            string[] commands = seperator.Split(input, 3);
            bool isDataActive = SceneManager.GetActiveScene().name != "BiosToLoad" && SceneManager.GetActiveScene().name != "ChoozeZip";
            if (!isDataActive)
            {
                return "Can't do this command now.";
            }
            if (!loginShortcut.interactable)
            {
                return "Can't do this command now.";
            }
            if (commands.Length < 2)
            {
                return "This command requires one argument.";
            }
            if (SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.Login))
            {
                login = SingletonMonoBehaviour<WindowManager>.Instance.GetWindowFromApp(AppType.Login).nakamiApp.GetComponent<Login>();
            }
            if (login != null)
            {
                login._passText.text = commands[1];
                password = login._passText.text;
            }
            else
            {
                password = commands[1];
            }
            if (password == "angelkawaii2" || password == "angelikawaii2")
            {
                SingletonMonoBehaviour<EventManager>.Instance.AddEvent<Scenario_loop1_day0_night_AfterLogin>();
                return "";
            }
            else
            {
                return "Wrong password!";
            }
        }
    }
}

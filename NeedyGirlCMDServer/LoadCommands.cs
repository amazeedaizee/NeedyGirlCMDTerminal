using ngov3;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

namespace NeedyGirlCMDServer
{
    internal class LoadCommands
    {
        internal static string ReloadSave()
        {
            int day;
            int user;
            App_LoadDataComponent restartLoad = new App_LoadDataComponent();
            DataPrefab dayLoad = new DataPrefab();
            Settings settings = SingletonMonoBehaviour<Settings>.Instance;
            bool isDataActive = SceneManager.GetActiveScene().name != "BiosToLoad" && SceneManager.GetActiveScene().name != "ChoozeZip";
            if (!isDataActive || settings.saveNumber < 0 || settings.saveNumber > 3 || !SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable)
            {
                return ErrorMessages.CMD_SPECIFIC_BUSY;
            }
            day = SingletonMonoBehaviour<StatusManager>.Instance.GetStatus(StatusType.DayIndex);
            user = settings.saveNumber;
            if (day == 1)
            {
                restartLoad._DataNumber = user;
                restartLoad.StartGame();
            }
            else
            {
                dayLoad.DataNum = user;
                dayLoad._datapath = $"Data{user}_Day{day}";
                dayLoad.Resume();
            }
            return "Loading day...";
        }
        internal static string LoadSave(string input)
        {
            App_LoadDataComponent restartLoad = new App_LoadDataComponent();
            DataPrefab dayLoad = new DataPrefab();
            var seperator = new Regex(@"\s+");
            string[] commands = seperator.Split(input, 4);
            if (!SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable)
                return ErrorMessages.CMD_SPECIFIC_BUSY;
            if (commands.Length == 1)
            {
                bool isWindowActive = SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.Jine);
                if (!isWindowActive)
                {
                    SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.LoadData);
                }
                else SingletonMonoBehaviour<WindowManager>.Instance.GetWindowFromApp(AppType.LoadData).Touched();
                return "";
            }
            if (commands.Length < 3)
            {
                return "This command when loading a specific day requires two arguments.";
            }
            if (!int.TryParse(commands[1], out int user))
            {
                return "Data must be a number.";
            }
            if (!int.TryParse(commands[2], out int day))
            {
                return "Day to load must be a number.";
            }
            if (!(user > 0 && user < 4))
            {
                return "Submitted user is not between 1 to 3.";
            }
            if (!(day > 0 && day < 31))
            {
                return "Submitted day is not between 1 to 30.";
            }
            if (day > 1 && !SaveRelayer.IsSlotDataExists($"Data{user}_Day{day}{SaveRelayer.EXTENTION}"))
            {
                return "Submitted user/day combination does not exist.";
            }
            if (day == 1)
            {
                restartLoad._DataNumber = user;
                restartLoad.StartGame();
            }
            else
            {
                dayLoad.DataNum = user;
                dayLoad._datapath = $"Data{user}_Day{day}";
                dayLoad.Resume();
            }
            return "Loading day...";
        }
    }
}

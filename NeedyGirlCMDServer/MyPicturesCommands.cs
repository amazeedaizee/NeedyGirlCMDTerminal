using NGO;
using ngov3;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

namespace NeedyGirlCMDServer
{
    internal class MyPicturesCommands
    {
        const string NO_IMG = "NoImagePicture";

        readonly static string[] backAction = { "back", "b" };
        readonly static string[] openAction = { "open", "o" };
        readonly static string[] goToAction = { "goto", "cd" };

        internal static List<ResourceLocal> resourceList;

        internal static string StartPicCommand(string input)
        {
            var seperator = new Regex(@"\s+");
            string[] commands = seperator.Split(input, 3);
            bool isWindowActive = SingletonMonoBehaviour<WindowManager>.Instance.isAppOpen(AppType.MyPicture);
            if (commands.Length == 1)
            {
                if (!isWindowActive)
                {
                    if ((SceneManager.GetActiveScene().name == "BiosToLoad" && !SingletonMonoBehaviour<Boot>.Instance.Login.interactable) ||
    (SceneManager.GetActiveScene().name != "ChooseZip" && SceneManager.GetActiveScene().name != "BiosToLoad" && !SingletonMonoBehaviour<TaskbarManager>.Instance._taskbarGroup.interactable))
                        return MsgManager.CMD_SPECIFIC_BUSY;
                    SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.MyPicture);
                }
                else SingletonMonoBehaviour<WindowManager>.Instance.GetWindowFromApp(AppType.MyPicture).Touched();
                return "";
            }
            else if (commands.Length == 3 && CommandManager.IsInputMatchCmd(commands[1], openAction))
            {
                return ViewPicture(commands[2]);
            }
            else
            {
                if (!isWindowActive) return "My Pictures window is not open!";
                var window = SingletonMonoBehaviour<WindowManager>.Instance.GetWindowFromApp(AppType.MyPicture);
                var picController = window.nakamiApp.GetComponent<MyPictureController_Sprit>();
                if (CommandManager.IsInputMatchCmd(commands[1], backAction))
                {
                    picController.ReturnToChooseFile();
                    return "";
                }
                if (commands.Length > 2)
                {
                    if (CommandManager.IsInputMatchCmd(commands[1], goToAction))
                    {
                        if (commands.Length == 2)
                            return "This command requires at least 3 arguments.";
                        else return ViewFolder(commands[2], picController);

                    }
                    else if (commands.Length == 2)
                    {
                        if (!(window._close.interactable || window._maximize.interactable || window._minimize.interactable))
                            return "Can't modify the My Pictures window now.";
                        if (WindowCommands.IsWindowScroll(window, commands[1]))
                        {
                            return "";
                        }
                        if (!WindowCommands.ChangeWindowState(window, commands[1]))
                        {
                            return "Invalid command for the My Pictures window.";
                        }
                        return "";

                    }

                }

            }
            return MsgManager.INVALID_CMD;
        }
        internal static string ViewPicture(string picName)
        {
            ResourceLocal image = resourceList.Find(i => i.Id == picName);
            var imageHistory = SingletonMonoBehaviour<Settings>.Instance.imageHistory;
            if (image == null)
            {
                return "Invalid image ID!";
            }
            else if (image != null && !imageHistory.Contains(image.FileName))
            {
                SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.ImageViewer).nakamiApp.GetComponent<ImageViewer>().SetData(NO_IMG);
            }
            SingletonMonoBehaviour<WindowManager>.Instance.NewWindow(AppType.ImageViewer).nakamiApp.GetComponent<ImageViewer>().SetData(image.FileName);
            return "";
        }

        internal static string ViewFolder(string name, MyPictureController_Sprit picController)
        {
            PictureFolder[] folders = picController.GetComponentsInChildren<PictureFolder>();
            if (!picController.chooseFolder.activeInHierarchy) picController.ReturnToChooseFile();
            foreach (var f in folders)
            {
                if (MatchFolder(name, f))
                {
                    if (f.isLocked) return "This folder has not been unlocked.";
                    else
                    {
                        picController.StartOpen(f);
                        return "";
                    }
                }
            }
            return "Could not find folder with that label!";
        }

        internal static bool MatchFolder(string name, PictureFolder f)
        {
            bool isMatch = false;
            int num = -1;
            string folderJP = NgoEx.SystemTextFromTypeString("PictureFolder", LanguageType.JP);
            string folderEN = NgoEx.SystemTextFromTypeString("PictureFolder", LanguageType.EN);
            string folderCN = NgoEx.SystemTextFromTypeString("PictureFolder", LanguageType.CN);
            string folderKO = NgoEx.SystemTextFromTypeString("PictureFolder", LanguageType.KO);
            string folderTW = NgoEx.SystemTextFromTypeString("PictureFolder", LanguageType.TW);
            string folderVN = NgoEx.SystemTextFromTypeString("PictureFolder", LanguageType.VN);
            string folderFR = NgoEx.SystemTextFromTypeString("PictureFolder", LanguageType.FR);
            string folderIT = NgoEx.SystemTextFromTypeString("PictureFolder", LanguageType.IT);
            string folderGE = NgoEx.SystemTextFromTypeString("PictureFolder", LanguageType.GE);
            string folderSP = NgoEx.SystemTextFromTypeString("PictureFolder", LanguageType.SP);
            string folderRU = NgoEx.SystemTextFromTypeString("PictureFolder", LanguageType.RU);
            if (int.TryParse(name, out num) && !(num < 0 || num > 30))
            {
                if (name == f.label.text)
                    return true;
            }
            if (Regex.IsMatch(name, @"^" + folderJP + @"$"))
                return true;
            if (Regex.IsMatch(name, @"^" + folderEN + @"$"))
                return true;
            if (Regex.IsMatch(name, @"^" + folderCN + @"$"))
                return true;
            if (Regex.IsMatch(name, @"^" + folderKO + @"$"))
                return true;
            if (Regex.IsMatch(name, @"^" + folderTW + @"$"))
                return true;
            if (Regex.IsMatch(name, @"^" + folderVN + @"$"))
                return true;
            if (Regex.IsMatch(name, @"^" + folderFR + @"$"))
                return true;
            if (Regex.IsMatch(name, @"^" + folderIT + @"$"))
                return true;
            if (Regex.IsMatch(name, @"^" + folderGE + @"$"))
                return true;
            if (Regex.IsMatch(name, @"^" + folderSP + @"$"))
                return true;
            if (Regex.IsMatch(name, @"^" + folderRU + @"$"))
                return true;
            if (Regex.IsMatch(name, @"^" + folderJP + @"\s{1}\d$"))
                isMatch = true;
            if (Regex.IsMatch(name, @"^" + folderEN + @"\s{1}\d$"))
                isMatch = true;
            if (Regex.IsMatch(name, @"^" + folderCN + @"\s{1}\d$"))
                isMatch = true;
            if (Regex.IsMatch(name, @"^" + folderKO + @"\s{1}\d$"))
                isMatch = true;
            if (Regex.IsMatch(name, @"^" + folderTW + @"\s{1}\d$"))
                isMatch = true;
            if (Regex.IsMatch(name, @"^" + folderVN + @"\s{1}\d$"))
                isMatch = true;
            if (Regex.IsMatch(name, @"^" + folderFR + @"\s{1}\d$"))
                isMatch = true;
            if (Regex.IsMatch(name, @"^" + folderIT + @"\s{1}\d$"))
                isMatch = true;
            if (Regex.IsMatch(name, @"^" + folderGE + @"\s{1}\d$"))
                isMatch = true;
            if (Regex.IsMatch(name, @"^" + folderSP + @"\s{1}\d$"))
                isMatch = true;
            if (Regex.IsMatch(name, @"^" + folderRU + @"\s{1}\d$"))
                isMatch = true;
            if (isMatch)
            {
                var seperator = new Regex(@"\s+");
                string[] commands = seperator.Split(name);
                var numEnd = commands[commands.Length - 1];
                if (f.label.text.EndsWith(numEnd)) return true;

            }
            return false;
        }
    }
}

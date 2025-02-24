using ngov3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NeedyGirlCMDServer
{
    internal class NotifCommands
    {
        internal static string ClickNotif()
        {
            Notification notif;
            RectTransform notifList;
            int notifIndex;
            bool isDataActive = SceneManager.GetActiveScene().name != "BiosToLoad" && SceneManager.GetActiveScene().name != "ChoozeZip";
            if (!isDataActive)
            {
                return MsgManager.CMD_SPECIFIC_BUSY;
            }
            var idea = SingletonMonoBehaviour<NetaManager>.Instance._chipGet;
            notifList = SingletonMonoBehaviour<NotificationManager>.Instance._notiferParent;
            if (notifList.childCount == 0 && idea._cover.alpha == 0f)
            {
                return "No notifications are active.";
            }
            if (idea._cover.alpha == 1f)
            {
                idea.NectButton.onClick.Invoke();
                return "";
            }
            notifIndex = SingletonMonoBehaviour<NotificationManager>.Instance._notiferParent.childCount - 1;
            notif = SingletonMonoBehaviour<NotificationManager>.Instance._notiferParent.GetChild(notifIndex).gameObject.GetComponent<Notification>();
            ExecuteEvents.Execute(notif.button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
            ExecuteEvents.Execute(notif.button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
            return "";
        }

        internal static string ClickToEnding()
        {
            IWindow endingDialog;
            var windowManager = SingletonMonoBehaviour<WindowManager>.Instance;
            var endingScreen = SingletonMonoBehaviour<EndingManager>.Instance;

            if (windowManager.isAppOpen(AppType.EndingDialog))
            {
                endingDialog = windowManager.GetWindowFromApp(AppType.EndingDialog);
                endingDialog.nakamiApp.GetComponent<EndingDialog>()._submitButton.onClick.Invoke();
                return "";
            }
            else if (endingScreen != null)
            {
                endingScreen.gameObject.GetComponent<Button>().onClick.Invoke();
                return "";
            }
            return MsgManager.CMD_SPECIFIC_BUSY;
        }
    }
}

using ngov3;

namespace NeedyGirlCMDServer
{
    internal class WristCommands
    {
        internal static string StartDestruct()
        {
            WristcutView ouch;
            WindowManager windowManager = SingletonMonoBehaviour<WindowManager>.Instance;
            if (!windowManager.isAppOpen(AppType.Darkness))
            {
                return MsgManager.SendMessage(ServerMessage.CMD_SPECIFIC_BUSY);
            }
            ouch = windowManager.GetWindowFromApp(AppType.Darkness).nakamiApp.GetComponentInChildren<WristcutView>();
            if (!ouch._goButton.gameObject.activeInHierarchy)
            {
                return MsgManager.SendMessage(ServerMessage.CMD_SPECIFIC_BUSY);
            }
            ouch._goButton.onClick.Invoke();
            return "";
        }
    }
}

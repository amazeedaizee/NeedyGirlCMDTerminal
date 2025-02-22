using Cysharp.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace NeedyGirlCMDServer
{
    internal class ConnectionManager
    {
        //internal static Thread thread;
        internal static TcpListener tcpListener = null;
        internal static bool isConnected = true;
        internal static TcpClient client = null;
        internal static void StartServer()
        {
            /// var currentUser = new SecurityIdentifier(WindowsIdentity.GetCurrent().User.Value);
            //var networkSid = new SecurityIdentifier(WellKnownSidType.NetworkSid, null);
            //PipeSecurity pipeSecurity = new PipeSecurity();
            //pipeSecurity.AddAccessRule(new PipeAccessRule(currentUser, PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow));
            //pipeSecurity.AddAccessRule(new PipeAccessRule(networkSid, PipeAccessRights.FullControl, System.Ssecurity.AccessControl.AccessControlType.Deny));
            //pipe = new NamedPipeServerStream("WindoseServer", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 0, 0, pipeSecurity);
            //thread = new Thread(WaitForConnection);
            //thread.Start();
            var port = 55770;
            IPAddress ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            if (tcpListener == null)
                tcpListener = new TcpListener(ipAddress, port);
            WaitForConnection().Forget();
        }

        internal static async UniTask WaitForConnection()
        {
            if (tcpListener == null)
            {
                Initializer.logger.LogInfo("Connection failed! Pipe is null.");
                return;
            }
            tcpListener.Start();
            Initializer.logger.LogInfo("Waiting...");
            client = await tcpListener.AcceptTcpClientAsync();
            Initializer.logger.LogInfo("boop");
            CommandManager.StartReceiveCommand().Forget();
            //thread.Join();


        }
    }
}

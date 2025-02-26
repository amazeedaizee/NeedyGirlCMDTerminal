using Cysharp.Threading.Tasks;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NeedyGirlCMDServer
{
    internal class ConnectionManager
    {
        internal static Thread thread;
        internal static TcpListener pipe;
        internal static TcpClient client;
        internal static CancellationTokenSource cts = new CancellationTokenSource();
        internal static bool isConnected = true;
        internal static void StartServer()
        {
            ///var currentUser = new SecurityIdentifier(WindowsIdentity.GetCurrent().User.Value);
            //var networkSid = new SecurityIdentifier(WellKnownSidType.NetworkSid, null);
            PipeSecurity pipeSecurity = new PipeSecurity();
            //pipeSecurity.AddAccessRule(new PipeAccessRule(currentUser, PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow));
            //pipeSecurity.AddAccessRule(new PipeAccessRule(networkSid, PipeAccessRights.FullControl, System.Security.AccessControl.AccessControlType.Deny));
            var linger = new LingerOption(false, 10);
            pipe = new TcpListener(IPAddress.Parse("127.0.0.1"), 55770);
            pipe.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, linger);
            pipe.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.MaxConnections, 1);
            //thread = new Thread(WaitForConnection);
            //thread.Start();
            WaitForConnection().Forget();
        }

        internal static async UniTask WaitForConnection()
        {
            if (pipe == null)
            {
                Initializer.logger.LogInfo("Connection failed! Pipe is null.");
                return;
            }
            Initializer.logger.LogInfo("Waiting...");
            pipe.Start();
            var linger = new LingerOption(false, 10);
            client = await pipe.AcceptTcpClientAsync();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, linger);
            //Initializer.logger.LogInfo("boop");
            CommandManager.StartReceiveCommand().Forget();
            //thread.Join();


        }


    }
}

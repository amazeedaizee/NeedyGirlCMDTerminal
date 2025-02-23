using Cysharp.Threading.Tasks;
using System.IO.Pipes;
using System.Threading;

namespace NeedyGirlCMDServer
{
    internal class ConnectionManager
    {
        internal static Thread thread;
        internal static NamedPipeServerStream pipe;
        internal static bool isConnected = true;
        internal static void StartServer()
        {
            ///var currentUser = new SecurityIdentifier(WindowsIdentity.GetCurrent().User.Value);
            //var networkSid = new SecurityIdentifier(WellKnownSidType.NetworkSid, null);
            PipeSecurity pipeSecurity = new PipeSecurity();
            //pipeSecurity.AddAccessRule(new PipeAccessRule(currentUser, PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow));
            //pipeSecurity.AddAccessRule(new PipeAccessRule(networkSid, PipeAccessRights.FullControl, System.Security.AccessControl.AccessControlType.Deny));
            pipe = new NamedPipeServerStream("WindoseServer", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 0, 0, pipeSecurity);
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

            await pipe.WaitForConnectionAsync();
            await UniTask.WaitUntil(() => { return pipe != null && pipe.IsConnected; });
            //Initializer.logger.LogInfo("boop");
            CommandManager.StartReceiveCommand().Forget();
            //thread.Join();


        }
    }
}

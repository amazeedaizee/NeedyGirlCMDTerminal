using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace NeedyGirlCMDTerminal
{
    internal class ConnectionManager
    {
        static string asciiArt = Resource.angelASCIIart;
        static bool skipArtLoad;
        internal static bool isRunning = true;
        internal static bool isNotConnected = true;

        internal static NamedPipeClientStream pipe;

        internal static void StartManualConnection()
        {
            try
            {
                pipe = new NamedPipeClientStream(".", "WindoseServer", PipeDirection.InOut, PipeOptions.Asynchronous, System.Security.Principal.TokenImpersonationLevel.Identification);
                pipe.Connect(1);
            }
            catch { }
        }

        internal static void StartLoad()
        {
            Task task = new Task(CancelArtLoad);
            task.Start();
            Console.Clear();
            Console.WriteLine("Connecting to the Windose service... \n\n");
            for (int i = 0; i < asciiArt.Length; i++)
            {
                if (!skipArtLoad)
                    Thread.Sleep(1);
                Console.Write(asciiArt[i]);
            }
            Console.WriteLine("\n");
            skipArtLoad = false;
            StartManualConnection();
#if !DEBUG
            if (!pipe.IsConnected)
            {
                pipe.Close();
                pipe.Dispose();
                pipe = null;
                Console.WriteLine("Connection failed!\nPress any key to try again.\n");
                Console.ReadKey(true);
                Console.Clear();
                StartLoad();
                return;
            }
#endif
            Console.WriteLine("Successfully connected to the Windose service!\n\n");
            Thread.Sleep(100);
            Console.WriteLine("NGO BIOS Rev1.0\n");
            Console.WriteLine("Main Processor : raincandy");
            Console.WriteLine("Memory Testing : OK\n\n");
            Thread.Sleep(500);
            CommandManager.CreateStreamReaderWriter();
        }

        internal static void ExitConsole(object sender, ConsoleCancelEventArgs e)
        {
            isRunning = false;
        }
        internal static void CancelArtLoad()
        {
            Console.ReadKey(true);
            skipArtLoad = true;
        }
    }
}

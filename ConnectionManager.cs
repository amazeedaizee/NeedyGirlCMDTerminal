using System;
using System.Net;
using System.Net.Sockets;
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

        internal static TcpClient link = new TcpClient();

        internal static void StartManualConnection()
        {
            try
            {
                link = new TcpClient();
                link.Connect(Dns.GetHostName(), 55770);
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
            if (!link.Connected)
            {
                link.Close();
                link.Dispose();
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

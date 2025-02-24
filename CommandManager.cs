using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NeedyGirlCMDTerminal
{
    public enum CommandState
    {
        AwaitingInput,
        ReadingInput,
        SendingOutput,
        TimedOut
    }

    internal class CommandManager
    {
        const int TEN_MINUTES = 60000 * 10;
        const string P_CHAN_INPUT = @"NGO:\Users\P> ";
        const string TIME_OUT_MSG = "Disconnected due to idling! Press the Enter/Return key to continue.\n";
        const string READ_OUT_MSG = "Disconnected due to no response from the server!\n";
        static string _currentCommand;
        static ConsoleKey restartKey;
        static StreamReader streamReader;
        static StreamWriter streamWriter;
        static CommandState state;

        internal static void CreateStreamReaderWriter()
        {
#if !DEBUG
            if (ConnectionManager.pipe != null && ConnectionManager.pipe.Connected)
            {
                streamReader = new StreamReader(ConnectionManager.ns);
                streamWriter = new StreamWriter(ConnectionManager.ns);
                streamWriter.AutoFlush = true;
            }
#else
            streamReader = StreamReader.Null;
            streamWriter = StreamWriter.Null;
#endif
            ConnectionManager.isRunning = true;
            Task.Run(WriteCommand);
            while (ConnectionManager.isRunning)
            {
                Thread.Sleep(100);
            }
            Console.WriteLine("Connection ended!");
            Console.WriteLine("Do you want to try and connect again? (Y/N)");
            do
            {
                restartKey = Console.ReadKey().Key;
                Thread.Sleep(100);
            }
            while ((restartKey != ConsoleKey.Y && restartKey != ConsoleKey.N) && !ConnectionManager.isRunning);
            if (restartKey == ConsoleKey.Y)
            {
                ConnectionManager.pipe.Close();
                ConnectionManager.pipe = null;
                ConnectionManager.StartLoad();
            }
            else Environment.Exit(0);
        }

        internal static void WriteCommand()
        {
            string command;
            var input = Console.In;
            state = CommandState.AwaitingInput;
            if (!ConnectionManager.isRunning)
                return;
            ConnectionManager.ns.WriteTimeout = 1;
            ConnectionManager.pipe.Client.Poll(-1, System.Net.Sockets.SelectMode.SelectWrite);
            if (!ConnectionManager.pipe.Connected)
            {
                ConnectionManager.isRunning = false;
                return;
            }
            ConnectionManager.ns.WriteTimeout = Timeout.Infinite;
            Task.Run(ExitCommandWrite);
            Task.Run(ReceiveCommand);
            Console.Write(P_CHAN_INPUT);
            _currentCommand = Console.ReadLine();
            if (state != CommandState.AwaitingInput)
            {
                ConnectionManager.isRunning = false;
                return;
            }
#if DEBUG
            Console.WriteLine("Command is " + _currentCommand);
#else
            SendCommand(_currentCommand);

#endif
            Thread.Sleep(100);
            WriteCommand();

        }

        internal static void SendCommand(string command)
        {
            if (!ConnectionManager.isRunning)
                return;
            if (!ConnectionManager.pipe.Connected)
            {
                ConnectionManager.isRunning = false;
                return;
            }
            streamWriter.WriteLine(command);
            state = CommandState.ReadingInput;
            Console.WriteLine($"Sent: {command}");
            //ConnectionManager.pipe.WaitForPipeDrain();
            while (state == CommandState.ReadingInput)
            {
                Thread.Sleep(100);
            }
        }

        internal static async Task ReceiveCommand()
        {
            int peek;
            string allMessages = "";
            string message = "";
            if (!ConnectionManager.isRunning)
                return;
            while (state != CommandState.ReadingInput)
            {
                Thread.Sleep(100);
                if (!ConnectionManager.pipe.Connected)
                {
                    ConnectionManager.isRunning = false;
                    return;
                }
            }
            Task.Run(ExitCommandRead);
            while ((message = await streamReader.ReadLineAsync()) != ">")
            {
                if (message == "!>")
                {
                    ConnectionManager.isRunning = false;
                    return;
                }
                if (message != ">")
                    allMessages += message;
                if (message != "?>")
                    Console.WriteLine(message);
                else
                {
                    IsHelpCommand(_currentCommand);
                    break;
                }


            }
            ConnectionManager.ns.ReadTimeout = Timeout.Infinite;
            state = CommandState.SendingOutput;
            streamReader.DiscardBufferedData();
            if (!ConnectionManager.pipe.Connected)
            {
                ConnectionManager.isRunning = false;
                return;
            }
            //if (state == CommandState.AwaitingInput)
            //    Console.Write(P_CHAN_INPUT);
        }


        internal static bool IsHelpCommand(string command)
        {
            ConsoleKey key;
            string helpFile;
            char[] seperator = { ' ' };
            string[] commands = command.Split(seperator, 3);
            if (commands[0] == "help")
            {
                helpFile = TableOfContents.GetHelpPage(commands);
                Console.WriteLine("\n++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                Console.WriteLine(helpFile);
                Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n");
            }
            return false;
        }
        internal static async Task ExitCommandWrite()
        {
            await Task.WhenAny(Task.Delay(TEN_MINUTES), CheckIfAwaitingInput());
            if (!ConnectionManager.isRunning)
                return;
            if (state != CommandState.AwaitingInput)
                return;
            Console.WriteLine("\n");
            Console.WriteLine(TIME_OUT_MSG);
            state = CommandState.TimedOut;

            async Task<bool> CheckIfAwaitingInput()
            {
                while (state == CommandState.AwaitingInput)
                {
                    await Task.Delay(1);
                }
                return true;
            }
        }

        internal static async Task ExitCommandRead()
        {
            ConnectionManager.ns.ReadTimeout = 2;
            await Task.WhenAny(Task.Delay(1), CheckIfReadingInput());
            if (!ConnectionManager.isRunning)
                return;
            if (state != CommandState.ReadingInput)
                return;
            ConnectionManager.pipe.Client.Poll(1, System.Net.Sockets.SelectMode.SelectRead);
            if (!ConnectionManager.pipe.Connected)
            {
                Console.WriteLine(READ_OUT_MSG);
                state = CommandState.TimedOut;
            }
            async Task<bool> CheckIfReadingInput()
            {
                while (state == CommandState.ReadingInput)
                {
                    await Task.Delay(1);
                }
                return true;
            }
        }
    }
}

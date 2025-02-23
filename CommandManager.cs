﻿using System;
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
        static string _currentCommand;
        static ConsoleKey restartKey;
        static StreamReader streamReader;
        static StreamWriter streamWriter;
        static CommandState state;

        internal static void CreateStreamReaderWriter()
        {
#if !DEBUG
            if (ConnectionManager.link != null && ConnectionManager.link.Connected)
            {
                streamReader = new StreamReader(ConnectionManager.link.GetStream());
                streamWriter = new StreamWriter(ConnectionManager.link.GetStream());
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
                ConnectionManager.link.Dispose();
                ConnectionManager.StartLoad();
            }
            else Environment.Exit(0);
        }

        internal static void WriteCommand()
        {
            string command;
            var input = Console.In;
            state = CommandState.AwaitingInput;
            if (!ConnectionManager.link.Connected)
            {
                ConnectionManager.isRunning = false;
            }
            if (!ConnectionManager.isRunning)
                return;
            //Task.Run(ExitCommandWrite);
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
            if (!ConnectionManager.link.Connected)
            {
                ConnectionManager.isRunning = false;
                return;
            }
            streamWriter.WriteLine(command);
            state = CommandState.ReadingInput;
            Console.WriteLine($"Sent: {command}");
            // ConnectionManager.link.GetStream().Flush();
            // ConnectionManager.link.WaitForPipeDrain();
            while (state == CommandState.ReadingInput)
            {
                Thread.Sleep(100);
            }
        }

        internal static async Task ReceiveCommand()
        {
            //  Console.WriteLine("Running Receive Command");
            string allMessages = "";
            string message = "";
            int count = 0;
            if (!ConnectionManager.isRunning)
                return;
            while (state != CommandState.ReadingInput)
            {
                Thread.Sleep(100);
                if (!ConnectionManager.link.Connected)
                {
                    ConnectionManager.isRunning = false;
                    return;
                }
            }
            do
            {
                if (!ConnectionManager.link.GetStream().DataAvailable) break;
                message = await streamReader.ReadLineAsync();
                allMessages += message;
                if (message != ">" && message != "?>")
                    Console.WriteLine(message);
            } while (ConnectionManager.link.Connected && message != ">" && message != "?>");
            if (allMessages == "?>")
            {
                IsHelpCommand(_currentCommand);
            }
            state = CommandState.SendingOutput;
            streamReader.DiscardBufferedData();
            if (!ConnectionManager.link.Connected)
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
            // Console.WriteLine("Running Exit Command");
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
    }
}

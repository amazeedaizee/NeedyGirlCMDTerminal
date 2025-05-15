using System;
using System.Management;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
namespace NeedyGirlCMDServer
{
    public static class NeofetchCommands
    {
        private static string LoadWindoseArt()
        {
            try
            {
                string artPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "windose_art.txt");
                if (File.Exists(artPath))
                {
                    return File.ReadAllText(artPath, Encoding.UTF8);
                }
                return "Windose Art Not Found\n";
            }
            catch (Exception ex)
            {
                return $"Error loading Windose art: {ex.Message}\n";
            }
        }

        public static string GetSystemInfo(string[] args)
        {
            try
            {
                string windoseArt = LoadWindoseArt();
                var osVersion = Environment.OSVersion;
                var machineName = Environment.MachineName;
                var userName = Environment.UserName;
                var processorCount = Environment.ProcessorCount;
                var uptime = TimeSpan.FromMilliseconds(Environment.TickCount);

                string cpuInfo = "N/A";
                try
                {
                    using (var searcher = new ManagementObjectSearcher("select Name from Win32_Processor"))
                    {
                        foreach (var mo in searcher.Get())
                        {
                            cpuInfo = mo["Name"]?.ToString() ?? "N/A";
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    cpuInfo = $"Error getting CPU info: {ex.Message}";
                }

                string totalMemory = "N/A";
                string availableMemory = "N/A";
                try
                {
                    ObjectQuery wql = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
                    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(wql))
                    {
                        ManagementObjectCollection results = searcher.Get();
                        foreach (ManagementObject result in results)
                        {
                            object totalVisibleObj = result["TotalVisibleMemorySize"];
                            object freePhysicalObj = result["FreePhysicalMemory"];

                            if (totalVisibleObj != null && freePhysicalObj != null)
                            {
                                double totalVisibleMemorySize = Convert.ToDouble(totalVisibleObj);
                                double freePhysicalMemory = Convert.ToDouble(freePhysicalObj);
                                totalMemory = $"{totalVisibleMemorySize / 1024.0 / 1024.0:F2} GB";
                                availableMemory = $"{freePhysicalMemory / 1024.0 / 1024.0:F2} GB";
                            }
                            else
                            {
                                totalMemory = "Error: RAM data missing";
                                availableMemory = "";
                            }
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    totalMemory = $"Error getting RAM info: {ex.Message}";
                    availableMemory = "";
                }
                
                var systemInfoString = $@"
User: {userName}@{machineName}
OS: Windose20
CPU: {cpuInfo} ({processorCount} cores)
RAM: {availableMemory} / {totalMemory}
Uptime: {uptime.Days}d {uptime.Hours}h {uptime.Minutes}m
";
                // Разделяем арт и системную информацию на строки
                string[] artLines = windoseArt.TrimEnd('\n').Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                string[] infoLines = systemInfoString.Trim().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                StringBuilder outputBuilder = new StringBuilder();
                int maxLines = Math.Max(artLines.Length, infoLines.Length);

                for (int i = 0; i < maxLines; i++)
                {
                    string artLine = (i < artLines.Length) ? artLines[i] : "";
                    string infoLine = (i < infoLines.Length) ? infoLines[i] : "";
                    
                   
                    outputBuilder.AppendLine($"{artLine.PadRight(40)} {infoLine}"); 
                }

                return outputBuilder.ToString().TrimStart();
            }
            catch (Exception e)
            {
                return $"Error fetching system info: {e.Message}";
            }
        }
    }
}

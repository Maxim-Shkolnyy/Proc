using System;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;


namespace Proc
{
    public class Program
    {
        static void Main(string[] args)
        {
            int processId = 0;

            //var processOrItsLibraryName = "explorer";
            var processOrItsLibraryName = "RPCRT4.dll";
            string monitoringToolsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ProcessMonitoring", ".\\");



            if (!processOrItsLibraryName.EndsWith(".dll"))
            {
                //Get process Id by process name
                using (Process processByName = Process.GetProcessesByName(processOrItsLibraryName)[0])
                {
                    processId = processByName.Id;
                    GetCommandLineArgsMOS(processId);
                }
            }
            else
            {
                //Get process Id by library name 
                var result = new List<ProcessInfo>();

                //foreach (var p in global::System.Diagnostics.Process.GetProcesses())
                //{
                //    Console.WriteLine(p.ProcessName);
                //    Console.WriteLine(p.Id);

                //    result.Add(new ProcessInfo
                //    {
                //        pid = p.Id,
                //        name = GetSafeFileName(p) ?? "System process. Access denied",
                //        title = p.MainWindowTitle,
                //        commandline = GetSafeCommandLine(p) ?? "System process. Access denied",
                //        //bitness = Environment.OSBitness == 32 || IsWOW64(p) ? 32 : 64
                //    });                    
                //}

                foreach (var p in global::System.Diagnostics.Process.GetProcesses())
                {
                    //Console.WriteLine($"Process Name: {p.ProcessName}");
                    //Console.WriteLine($"Process ID: {p.Id}");

                    var processInfo = new ProcessInfo
                    {
                        pid = p.Id,
                        name = GetSafeFileName(p) ?? "System process. Access denied",
                        title = p.MainWindowTitle,
                        commandline = GetSafeCommandLine(p) ?? "System process. Access denied",
                        //bitness = Environment.OSBitness == 32 || IsWOW64(p) ? 32 : 64
                    };

                    // Додаємо до списку
                    result.Add(processInfo);

                    // Виводимо інформацію про доданий об'єкт
                    Console.WriteLine($"Added Process Info:");
                    Console.WriteLine($"PID: {processInfo.pid}");
                    Console.WriteLine($"Name: {processInfo.name}");
                    Console.WriteLine($"Title: {processInfo.title}");
                    Console.WriteLine($"Command Line: {processInfo.commandline}");
                    Console.WriteLine(new string('-', 50));
                }


                //Process process;
                //string name = "";
                //foreach (Process p in Process.GetProcesses())
                //{
                //    process = p;
                //    name = process.ProcessName;
                //    try
                //    {
                //        if (process.Modules.Cast<ProcessModule>().Any(m => m.ModuleName.Equals(processOrItsLibraryName, StringComparison.OrdinalIgnoreCase)))
                //        {
                //            processId = process.Id;
                //            Console.WriteLine($"Process: {process.ProcessName}, ID: {processId} uses {processOrItsLibraryName}");

                //            result.Add(new ProcessInfo
                //            { 
                //            pid = p.Id, /*user =, state = split[2],*/
                //                name = length(p.StartInfo.FileName) > 0 ? (p.StartInfo.FileName as string)?.ToUnixPathFromWindowsPath : p.ProcessName,
                //                title = p.MainWindowTitle,
                //            commandline = p.StartInfo.Arguments,
                //            //bitness = Environment.OSBitness == 32 || IsWOW64(p) ? 32 : 64  
                //            });

                //            Console.WriteLine(result.Count);
                //        }
                //            //#endif
                //            //return result;                           

                //    }
                //    catch (System.ComponentModel.Win32Exception ex) //  when (ex.Message.Contains("Unable to enumerate the process modules"))
                //    {
                //        if (processId < 1)
                //        {
                //            processId = GetProcessIdUsingPsList(name, monitoringToolsPath);
                //            GetCommandLineArgsMOS(processId);
                //            string psListCommand = $"{Path.Combine(monitoringToolsPath, "pslist.exe")} {process.ProcessName} -accepteula";
                //            //string listDLLsCommand = $"{Path.Combine(monitoringToolsPath, "ListDLLs.exe")} -p {processId} -accepteula";

                //            GetSystemProcessInfo(psListCommand);
                //            //GetSystemProcessInfo(listDLLsCommand);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine($"Unable to access process info: {ex.Message} ProcessId: {processId}. Process name: {process}");                       
                //    }
                //    processId = 0;
                //}
            }
        }


        private static string GetSafeFileName(Process process)
        {
            try
            {
                return !string.IsNullOrEmpty(process.StartInfo.FileName)
                    ? process.StartInfo.FileName //.ToUnixPathFromWindowsPath()
                    : process.ProcessName;
            }
            catch (Exception ex)
            {
                return process.ProcessName;
            }
        }


        private static string? GetSafeCommandLine(Process process)
        {
            var processId = process.Id;
            try
            {
                return process.StartInfo.Arguments;
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                return GetCommandLineArgsMOS(processId);
            }
            catch (InvalidOperationException ex)
            {
                return GetCommandLineArgsMOS(processId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error for Arguments: {ex.Message}");
            }
            return "Unexpected error for process Arguments";
        }



        private static string GetCommandLineArgsMOS(int processId)
        {
            string query = $"SELECT * FROM Win32_Process WHERE ProcessId = {processId}";

            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    return obj["CommandLine"]?.ToString() ?? "Command line not available";                    

                    //Console.WriteLine($"Process ID: {processId}");
                    //foreach (var property in obj.Properties)
                    //{
                    //    Console.WriteLine($"{property.Name}: {property.Value}");
                    //}
                    //Console.WriteLine(new string('-', 50));
                }
            }
            return "Command line not available";
        }


        private static int GetProcessIdUsingPsList(string processName, string monitoringToolsPath)
        {
            string psListCommand = $"{Path.Combine(monitoringToolsPath, "pslist.exe")} {processName} -accepteula";
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {psListCommand}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        throw new InvalidOperationException("Failed to start pslist process.");
                    }

                    using (var reader = process.StandardOutput)
                    {
                        string output = reader.ReadToEnd();

                        string[] lines = output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string line in lines)
                        {
                            if (line.StartsWith(processName, StringComparison.OrdinalIgnoreCase))
                            {
                                string[] columns = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                if (columns.Length > 1 && int.TryParse(columns[1], out int processId))
                                {
                                    return processId;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving process ID with pslist: {ex.Message}");
            }

            return -1;
        }


        private static void GetSystemProcessInfo(string command)
        {
            try
            {
                using (Process cmdProcess = new Process())
                {
                    cmdProcess.StartInfo.FileName = "cmd.exe";
                    cmdProcess.StartInfo.Arguments = $"/C {command}";
                    cmdProcess.StartInfo.RedirectStandardOutput = true;
                    cmdProcess.StartInfo.RedirectStandardError = true;
                    cmdProcess.StartInfo.UseShellExecute = false;
                    cmdProcess.StartInfo.CreateNoWindow = true;

                    cmdProcess.Start();

                    string output = cmdProcess.StandardOutput.ReadToEnd();
                    string error = cmdProcess.StandardError.ReadToEnd();

                    Console.WriteLine(output);

                    if (!string.IsNullOrWhiteSpace(error))
                        Console.WriteLine($"Error: {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Command execution failed: {ex.Message}");
            }
        }


        private static int GetProcessId(Process process)
        {
            string query = "SELECT ProcessId, Handle FROM Win32_Process";
            using (var searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj["Handle"] != null && Convert.ToInt32(obj["Handle"]) == process.Handle.ToInt32())
                    {
                        return Convert.ToInt32(obj["ProcessId"]);
                    }
                }
            }
            return -1;
        }


        private static void ExecuteSysinternalsTool(string toolPath, string toolName, string arguments)
        {
            string fullPath = Path.Combine(toolPath, toolName);
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = fullPath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string output = reader.ReadToEnd();
                    Console.WriteLine(output);
                }

                using (StreamReader reader = process.StandardError)
                {
                    string errorOutput = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(errorOutput))
                    {
                        Console.WriteLine($"Error: {errorOutput}");
                    }
                }

                process.WaitForExit();
            }
        }


        private static void CopyCurrentProcessProperties()
        {
            var currentEnvVars = Environment.GetEnvironmentVariables();

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "notepad.exe",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            foreach (var key in currentEnvVars.Keys)
            {
                startInfo.Environment[key.ToString()] = currentEnvVars[key].ToString();
            }

            try
            {
                Process newProcess = Process.Start(startInfo);
                Console.WriteLine("Started new process with copied environment variables.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting process: {ex.Message}");
            }
        }
    }
}

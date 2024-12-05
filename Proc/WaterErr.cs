using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Proc
{
    public class ProcessInfo
    {
        public string name { get; set; }
        public int pid { get; set; }
        public string user { get; set; }
        public string state { get; set; }
        public string title { get; set; }
        public string commandline { get; set; }
        public int? bitness { get; set; }
        public string? architecture { get; set; }
    }
    public class WaterErr
    {
        //public override ImmutableList<ProcessInfo> GetActiveProcessList(CrossBoxDevice? device = null)
        //{
        //    if (device == null || device.IsLocalComputer)
        //        return getActiveProcessListWindows();
        //    return null;
        //}

        //private ImmutableList<ProcessInfo> getActiveProcessListWindows()
        //{
        //    var result = new List<ProcessInfo>();

        //    try
        //    {
        //        //#if ECHOES
        //        foreach (var p in global::System.Diagnostics.Process.GetProcesses())
        //        {
        //            result.Add(new ProcessInfo
        //            {
        //                pid = p.Id, /*user =, state = split[2],*/
        //                name = length(p.StartInfo.FileName) > 0 ? (p.StartInfo.FileName as string)?.ToUnixPathFromWindowsPath : p.ProcessName,
        //                title = p.MainWindowTitle,
        //                commandline = p.StartInfo.Arguments,
        //                bitness = Environment.OSBitness == 32 || IsWOW64(p) ? 32 : 64
        //            });
        //        }
        //        //#endif
        //        return result;
        //    }
        //    catch(System.ComponentModel.Win32Exception ex)
        //    {
            
        //    }
        //}

        //#if ECHOES
        //private static bool IsWOW64(global::System.Diagnostics.Process process)
        //{
        //    try
        //    {
        //        if ((global::System.Environment.OSVersion.Version.Major > 5) || ((global::System.Environment.OSVersion.Version.Major == 5) && (global::System.Environment.OSVersion.Version.Minor >= 1)))
        //        {
        //            bool retVal;
        //            return NativeMethods.IsWow64Process(process.Handle, out retVal) && retVal;
        //        }
        //        return false; // not on 64-bit Windows Emulator
        //    }
        //    catch (global::System.ComponentModel.Win32Exception ex)
        //    {
        //        if (ex.NativeErrorCode != 0x00000005)
        //            throw;
        //    }
        //    catch (global::System.InvalidOperationException ex)
        //    {
        //        return false;
        //    }
        //}
    }
}

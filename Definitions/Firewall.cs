using System.Diagnostics;
using static Methods;

namespace Qwarp.Definitions;
public static class Firewall
{
    public static bool Block(string IP)
    {
        using var Process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Linux ? "sudo" : "netsh",
                Arguments = Linux ? $"ip route add blackhole {IP}"
                : $"advfirewall firewall add rule name=\"Qwarp {IP}\" dir=in,out action=block remoteip={IP} enable=yes",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };

        try
        {
            Process.Start();
            string Error = Process.StandardError.ReadToEnd();
            string Output = Process.StandardOutput.ReadToEnd();
            Process.WaitForExit();
            
            return (Process.ExitCode == 0);
        }
        catch 
        {
            return false;
        }
    }

    public static bool Unblock(string IP)
    {
        using var Process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Linux ? "sudo" : "netsh",
                Arguments = Linux ? $"ip route delete blackhole {IP}" 
                : $"advfirewall firewall delete rule name=\"Qwarp {IP}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };

        try
        {
            Process.Start();
            string Error = Process.StandardError.ReadToEnd();
            string Output = Process.StandardOutput.ReadToEnd();
            Process.WaitForExit();
            
            return (Process.ExitCode == 0);
        }
        catch 
        {
            return false;
        }
    }
}
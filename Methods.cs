using System.Net;
using System.Text.Json;
using System.Runtime.InteropServices;
using Qwarp.Utility;

public static class Methods
{
    public static void Write(string? str) => Console.WriteLine(str);
    public static bool IsIP(string str) => IPAddress.TryParse(str, out _);

    public static int Parallels = 10;
    public static string Platform = RuntimeInformation.OSDescription;
    public static bool Linux = Platform.Contains("Linux");
    public static string Base = AppDomain.CurrentDomain.BaseDirectory;

    public static JsonSerializerOptions PrettyPrint = new JsonSerializerOptions { WriteIndented = true };
    public static ParallelOptions ParallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Parallels };
    public static StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;
    public static string ServersJson = Path.Combine(Base, "servers.json");
    public static string FilterJson = Path.Combine(Base, "filter.json");
    public static bool Silent = false;

    public static void Help()
    {
        Write("Usage: qwarp [argument] [params]");
        Write("");
        Write("list\t\tList all servers");
        Write("refresh\t\tFetch latest servers data");
        Write("block\t\t[Server name/country/index]");
        Write("unblock\t\t[Server name/country/index]");
        Write("");
        Write("blackhole\tBlock every server");
        Write("unwarp\t\tUndo all changes");
        Write("");
        Terminal.Out("[i]Additional commands for wrappers:");
        Write("raw-servers\tFetch and print latest servers JSON (raw)");
        Write("block-ips\t[IP1,IP2,IP3...]");
        Write("unblock-ips\t[IP1,IP2,IP3...]");
        Environment.Exit(1);
    }

    public static void Log(string Status, string Message)
    {
        var Log = new { status = Status, message = Message };
        Write(Log.ToString());
    }
}
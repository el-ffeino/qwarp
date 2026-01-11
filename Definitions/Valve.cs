using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Diagnostics;
using Qwarp.Utility;
using static Methods;

namespace Qwarp.Definitions;
public static class Valve
{
    private const string Endpoint = "https://api.steampowered.com/ISteamApps/GetSDRConfig/v1/?appid=730";
    private const string UserAgent = "Mozilla/5.0 (X11; Linux x86_64; rv:146.0) Gecko/20100101 Firefox/146.0";

    public static async Task<List<Server>> GetServers()
    {
        for (int Attempts = 3; Attempts != 0; Attempts--)
        {
            using var Client = new HttpClient();
            Client.Timeout = TimeSpan.FromSeconds(5);
            Client.DefaultRequestHeaders.UserAgent.TryParseAdd(UserAgent);

            try
            {
                string Response = await Client.GetStringAsync(Endpoint);
                if (string.IsNullOrEmpty(Response))
                {
                    Thread.Sleep(1000);
                    continue;
                }

                JsonNode? Root = JsonNode.Parse(Response);
                if (Root is null) continue;

                JsonNode? Pops = Root["pops"];
                if (Pops is null) continue;

                var Servers = new List<Server>();
                var InvalidServers = new ConcurrentBag<Server>();

                foreach (var Property in Pops.AsObject())
                {
                    string Key = Property.Key;

                    JsonNode? Node = Property.Value;
                    if (Node is null) continue;

                    string Name = Node["desc"]?.ToString() ?? Key;
                    if (string.IsNullOrEmpty(Name)) continue;
                    Log("Info", $"Found server: {Name}");

                    JsonNode? RelaysNode = Node["relays"];
                    if (RelaysNode is null) continue;

                    JsonArray? Relays = RelaysNode.AsArray();
                    if (Relays?.Count == 0) continue;

                    var Server = new Server(Name.Trim());
                    var IPs = new List<string>();

                    foreach (JsonNode? Relay in Relays!.AsArray())
                    {
                        string? IP = Relay!["ipv4"]?.GetValue<string>();
                        if (!string.IsNullOrEmpty(IP))
                        {
                            Server.IPs.Add(IP);
                        }
                    }

                    if (Server.IPs.Count > 0)
                    {
                        Servers.Add(Server);
                    }
                }

                var Options = new ParallelOptions { MaxDegreeOfParallelism = Parallels };
                await Parallel.ForEachAsync(Servers, Options, async (Server, CancelToken) => 
                {
                    Server.EstimatePing();

                    if (Server.Ping == -1)
                        InvalidServers.Add(Server);
                });

                foreach (var Server in InvalidServers)
                {
                    Servers.Remove(Server);
                    Log("Warning", $"Unreachable server: {Server.Name}");
                }

                return Servers.OrderBy(s => s.Ping).ToList();
            }
            catch {}
        }

        return new List<Server>();
    }

    public static int Ping(string IP, int Timeout = 1000)
    {
        if (string.IsNullOrWhiteSpace(IP)) return -1;

        try
        {
            using (Ping Ping = new Ping())
            {
                PingReply Reply = Ping.Send(IP, Timeout);
                if (Reply.Status == IPStatus.Success)
                {
                    return (int)Reply.RoundtripTime;
                }
            }
        }
        catch {}

        return -1;
    }
}

public class Server
{
    public string Name { get; set; }
    public string Country { get; set; }
    public bool Blocked { get; set; }
    public int Ping { get; set; }
    public List<string> IPs { get; set; }

    public Server(string name)
    {
        int CountryStart = name.LastIndexOf(" (");
        int CountryEnd = name.LastIndexOf(')');

        if (CountryStart is -1 || CountryEnd is -1)
        {
            Name = name;
            Country = name;
        }
        else
        {
            Name = name.Substring(0, CountryStart).Trim();
            Country = name.Substring(CountryStart + 2, CountryEnd - CountryStart - 2).Trim();
        }

        IPs = new List<string>();
        Ping = -1;
        Blocked = false;
    }

    public void EstimatePing()
    {
        foreach (string IP in IPs)
        {
            int Latency = Valve.Ping(IP);

            if (Latency != -1)
            {
                if (Ping == -1)
                {
                    Ping = Latency;
                }
                else
                {
                    Ping -= (Ping - Latency) / 2;
                }
            }
        }
    }
}
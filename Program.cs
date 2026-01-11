using System.IO;
using System.Text.Json;
using System.Threading;
using Qwarp.Definitions;
using Qwarp.Utility;
using static Methods;

var Servers = new List<Server>();
var Filter = new List<string>();
await Initialize();

if (args.Length < 1)
{
    if (Linux)
    {
        if (Filter.Count is 0)
        {
            Methods.Help();
        }

        await Parallel.ForEachAsync(Filter, Methods.ParallelOptions, async (IP, CancelToken) => Firewall.Block(IP));
        Environment.Exit(0);
    }
    else Methods.Help();
}

string Command = args[0];
switch (Command)
{
    case "list":
    {
        for (int i = 0; i < Servers.Count; i++)
        {
            var Server = Servers[i];

            string Index = Server.Blocked ? $"[[b][r]{i}[/r]]" : $"[[b][accent]{i}[/r]]";
            string Ping = $"[g]{Server.Ping}";

            if (Server.Ping > 65)
            {
                Ping = $"[y]{Server.Ping}";
            }
            if (Server.Ping > 145)
            {
                Ping = $"[r]{Server.Ping}";
            }

            Terminal.Out($"{Index} {Server.Name} ({Server.Country}) {Ping}ms");
        }
    }
    break;
    case "refresh":
    {
        var servers = await Valve.GetServers();

        foreach (var Server in servers)
        {
            var Existing = Servers.FirstOrDefault(s => s.Name.Equals(Server.Name, IgnoreCase));

            if (Existing is null) 
                Servers.Add(Server);
            else
            {
                Existing.Ping = Server.Ping;
            }
        }

        Servers = Servers.OrderBy(s => s.Ping).ToList();
    }
    break;
    case "block" or "unblock":
    {
        var IPs = new List<string>();
        bool Block = (Command is "block");

        if (args.Length < 2)
        {
            Terminal.Error("Missing second argument.");
            Environment.Exit(1);
        }

        // By index
        if (int.TryParse(args[1], out int Index))
        {
            try
            {
                var Server = Servers[Index];

                Server.Blocked = Block;
                IPs.AddRange(Server.IPs);
            }
            catch
            {
                Terminal.Error($"Server with index '{Index}' does not exist.");
                Environment.Exit(1);
            }
        }
        else
        {
            var Server = Servers.FirstOrDefault(s => s.Name.Equals(args[1], IgnoreCase));

            // By country
            if (Server is null)
            {
                var Matches = Servers.Where(s => s.Country.Equals(args[1], IgnoreCase)).ToList();

                if (!Matches.Any())
                {
                    Terminal.Error($"There are no servers matching '{args[1]}'");
                    Environment.Exit(1);
                }

                foreach (var Match in Matches)
                {
                    IPs.AddRange(Match.IPs);
                    Match.Blocked = Block;
                }
            }
            // By name
            else
            {
                IPs.AddRange(Server.IPs);
                Server.Blocked = Block;
            }
        }

        if (Block)
        {
            Filter.AddRange(IPs);
        }
        else
        {
            Filter.RemoveAll(IP => IPs.Contains(IP));
        }

        await Parallel.ForEachAsync(IPs, Methods.ParallelOptions, async (IP, CancelToken) =>
        {
            if (Block)
            {
                Firewall.Block(IP);
            }
            else
            {
                Firewall.Unblock(IP);
            }
        });
    }
    break;

    case "blackhole":
    {
        var IPs = new List<string>();

        foreach (var Server in Servers.Where(s => !s.Blocked))
        {
            Server.Blocked = true;
            IPs.AddRange(Server.IPs);
            Filter.AddRange(Server.IPs);
        }

        await Parallel.ForEachAsync(IPs, Methods.ParallelOptions, async (IP, CancelToken) =>
        {
            Firewall.Block(IP);
        });
    }
    break;
    case "unwarp":
    {
        var IPs = new List<string>();

        foreach (var Server in Servers.Where(s => s.Blocked))
        {
            Server.Blocked = false;
            IPs.AddRange(Server.IPs);
            Filter.RemoveAll(IP => Server.IPs.Contains(IP));
        }

        await Parallel.ForEachAsync(IPs, Methods.ParallelOptions, async (IP, CancelToken) =>
        {
            Firewall.Unblock(IP);
        });
    }
    break;

    case "raw-servers":
    {
        var Latest = await Valve.GetServers();
        Write(JsonSerializer.Serialize(Latest));
    }
    break;
    case "block-ips" or "unblock-ips":
    {
        bool Block = (Command is "block-ips");

        if (args.Length < 2)
        {
            Terminal.Error("Missing second argument.");
            Environment.Exit(1);
        }

        var IPs = args[1].Split(",").ToList();

        await Parallel.ForEachAsync(IPs, Methods.ParallelOptions, async (IP, CancelToken) =>
        {
            if (Block)
                Firewall.Block(IP);
            else
                Firewall.Unblock(IP);
        });
    }
    break;

    default:
    {
        Methods.Help();
    }
    break;
}

if (!IsWrapperCommand(Command))
{
    File.WriteAllText(ServersJson, JsonSerializer.Serialize(Servers, PrettyPrint));
    File.WriteAllText(FilterJson, JsonSerializer.Serialize(Filter, PrettyPrint));
}

// Main methods
async Task Initialize()
{
    if (!File.Exists(ServersJson))
    {
        Terminal.Warning("Server list not found, fetching latest..");
        Servers = await Valve.GetServers();
        File.WriteAllText(ServersJson, JsonSerializer.Serialize(Servers, PrettyPrint));
    }

    Servers = JsonSerializer.Deserialize<List<Server>>(File.ReadAllText(ServersJson));

    if (!File.Exists(FilterJson))
    {
        File.WriteAllText(FilterJson, JsonSerializer.Serialize(Filter, PrettyPrint));
    }

    Filter = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(FilterJson));
}

bool IsWrapperCommand(string Command)
{
    var WrapperCommands = new string[] { "raw-servers", "block-ips", "unblock-ips" };
    return WrapperCommands.Contains(Command);
}
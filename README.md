# qwarp

Counter-Strike 2 matchmaking server chooser.  
**Qwarp** *(Queue Warp)* is a cross-platform command-line program that allows you to block specific Counter-Strike 2 (Valve) servers.  

## ⁉️ How does it work?
It isolates the servers you want to play on by blocking the rest inside your system's firewall and works on both Windows and Linux systems.   

#### Usage
```console
qwarp [command] [parameters]
```

#### Commands
```console
list                                        View your current configuration
refresh                                     Fetch latest Valve servers' data
block [Server name / country / index]       Block a specific server
unblock [Server name / country / index]     Unblock a specific server

blackhole                                   Block all servers at once
unwarp                                      Revert all changes

- Additional commands (ignores config)    
raw-servers                                 Fetch and print latest servers JSON (raw) data
block-ips [IP-1,IP-2,IP-3...]               Block multiple IPs at once
unblock-ips [IP-1,IP-2,IP-3...]             Unblock multiple IPs at once                                 
```

##### Example usage
Let's say you wanted to play on Frankfurt, Germany server specifically:  
Keep in mind **super-user (administrator) privileges are required** to change Firewall settings!  

`qwarp blackhole && qwarp unblock frankfurt`  

  
> [!WARNING]
> Steam Networking may still find a game in another server by routing it throught the selected server.  
> To get around this, you can set `Options -> Game -> Maximum acceptable ping` to match the server(s) you've selected.  
> (Unblocked servers will always have lower ping since there's no relays between.)

## 📦 Dependencies
Program is written in C#, therefore you'll need [.NET Runtime](https://dotnet.microsoft.com/en-us/download) to run it.  

> [!NOTE]
> Windows users likely already have it installed, but if not - get [.NET runtime from Microsoft](https://dotnet.microsoft.com/en-us/download)

**Arch Linux**:  
```console
sudo pacman -S dotnet-runtime
```

## 🛠 Installation
- Get the latest [release](https://github.com/el-ffeino/qwarp/releases) for either Linux or Windows
- Unpack and you're good to go  

#### Linux
- Make sure the binary is executable with `chmod +x qwarp`.

Since Firewall changes are reset on reboot, you might want to make it autorun on start.  
Create `qwarp.service` file with the following contents:
```
[Service]
WorkingDirectory=/
ExecStart=
```

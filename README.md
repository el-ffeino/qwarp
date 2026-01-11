# qwarp

Counter-Strike 2 matchmaking server chooser.  

**Qwarp** *(Queue Warp)* is a cross-platform command-line program that allows you to block specific Counter-Strike 2 (Valve) servers.  
It works by blocking the servers you don't want to play on within your system's Firewall on **Windows** and **Linux**.  

## ⁉️ How do I use it?
#### Usage
```console
qwarp [command] [parameters]
```

#### Commands
```
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
Keep in mind **superuser (administrator) privileges are required** to change Firewall settings!  

```console
qwarp blackhole  
qwarp unblock frankfurt
```  

  
> [!NOTE]
> Steam Networking may still find a game in another server by routing it throught the selected server.  
> To get around this, you can set `Options -> Game -> Maximum acceptable ping` to match the server(s) you've selected.  
> (Unblocked servers will always have lower ping since there's no relays between)

## 📦 Dependencies
Program is written in C#, therefore you'll need [.NET Runtime](https://dotnet.microsoft.com/en-us/download) to run it.  
Windows users likely already have it installed, but if not: [Download from Microsoft](https://dotnet.microsoft.com/en-us/download)!

**Arch Linux**:  
```console
sudo pacman -S dotnet-runtime
```

## 🛠 Installation
- Get the latest [release](https://github.com/el-ffeino/qwarp/releases) for either Linux or Windows
- Unpack and you're good to go  

#### Notes for Linux users
- Make sure the binary is executable with `chmod +x qwarp`.  
- Firewall changes are reset on reboot, you might be interested in making Qwarp run on startup.  

### Is it bannable?
Qwarp does **NOT** interact with the game's files or memory in any way; However, I can't confirm whether Valve allows these kind of programs to be used or not.  
**Use at your own risk!**

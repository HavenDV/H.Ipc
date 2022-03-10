# [H.Ipc](https://github.com/HavenDV/H.Ipc/) 

[![Language](https://img.shields.io/badge/language-C%23-blue.svg?style=flat-square)](https://github.com/HavenDV/H.Ipc/search?l=C%23&o=desc&s=&type=Code)
[![License](https://img.shields.io/github/license/HavenDV/H.Ipc.svg?label=License&maxAge=86400)](LICENSE.md)
[![Build Status](https://github.com/HavenDV/H.Ipc/workflows/.NET/badge.svg?branch=master)](https://github.com/HavenDV/H.Ipc/actions?query=workflow%3A%22.NET%22)
[![NuGet](https://img.shields.io/nuget/dt/H.Ipc.svg?style=flat-square&label=H.Ipc)](https://www.nuget.org/packages/H.Ipc/)

### Usage
```cs
// Common interface
public interface IActionService
{
    void ShowTrayIcon();
    void HideTrayIcon();
}

// Server side implementation
[H.IpcGenerators.IpcServer]
public partial class ActionService : IActionService
{
    public void ShowTrayIcon()
    {
        MessageBox.Show(nameof(ShowTrayIcon));
    }

    public void HideTrayIcon()
    {
        MessageBox.Show(nameof(HideTrayIcon));
    }
}

// Client side implementation
[H.IpcGenerators.IpcClient]
public partial class ActionServiceClient : IActionService
{
}

// Server initialization
var server = new PipeServer<string>(ServerName);
var service = new ActionService();
service.Initialize(server);
await server.StartAsync();

// Client initialization
var client = new PipeClient<string>(ServerName);
var service = new ActionServiceClient();
service.Initialize(pipeClient);
await server.ConnectAsync();

// Client usage
client.ShowTrayIcon();
```

### Contacts
* [mail](mailto:havendv@gmail.com)

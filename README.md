# H.Ipc.Generator

### Nuget
[![NuGet](https://img.shields.io/nuget/dt/H.Ipc.Generator.svg?style=flat-square&label=H.Ipc.Generator)](https://www.nuget.org/packages/H.Ipc.Generator/)
```
Install-Package H.Ipc.Generator
```

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

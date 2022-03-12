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
    void SendText(string text);
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

    public void SendText(string text)
    {
        MessageBox.Show(text);
    }
}

// Client side implementation
[H.IpcGenerators.IpcClient]
public partial class ActionServiceClient : IActionService
{
}

// Server initialization
await using var server = new PipeServer<string>(ServerName);
var service = new ActionService();
service.Initialize(server);
await server.StartAsync();

// Client initialization
await using var client = new PipeClient<string>(ServerName);
var service = new ActionServiceClient();
service.Initialize(client);
await client.ConnectAsync();

// Client usage
client.ShowTrayIcon();
```

### Contacts
* [mail](mailto:havendv@gmail.com)

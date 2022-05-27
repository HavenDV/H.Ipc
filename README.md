# H.Ipc
This generator allows you to generate boilerplate code for [H.Pipes](https://github.com/HavenDV/H.Pipes) based on the interface you specify.
Generation example: https://github.com/HavenDV/H.ProxyFactory/issues/7#issuecomment-1072287342

### Nuget
[![NuGet](https://img.shields.io/nuget/dt/H.Ipc.Generator.svg?style=flat-square&label=H.Ipc.Generator)](https://www.nuget.org/packages/H.Ipc.Generator/)
```
Install-Package H.Ipc.Generator
Install-Package H.Ipc.Core
Install-Package H.Pipes
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

### Notes
The generated code currently requires C# version 8 and above. You can enable this using the following code in your .csproj file:
```xml
<PropertyGroup>
  <LangVersion>preview</LangVersion> <!-- or just 8.0 -->
</PropertyGroup>
```

### Contacts
* [mail](mailto:havendv@gmail.com)

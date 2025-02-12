# H.Ipc
This generator allows you to generate boilerplate code for [H.Pipes](https://github.com/HavenDV/H.Pipes) based on the interface you specify.
Generation example: https://github.com/HavenDV/H.ProxyFactory/issues/7#issuecomment-1072287342

### Nuget
[![NuGet](https://img.shields.io/nuget/dt/H.Ipc.svg?style=flat-square&label=H.Ipc)](https://www.nuget.org/packages/H.Ipc/)
```
Install-Package H.Ipc
```

### Usage
```cs
// Common interface
public interface IActionService
{
    Task ShowTrayIcon();
    Task HideTrayIcon();
    Task SendText(string text);
    Task<int> GetAnswer(string question);
}

// Server side implementation
[H.IpcGenerators.IpcServer]
public partial class ActionService : IActionService
{
    public Task ShowTrayIcon()
    {
        MessageBox.Show(nameof(ShowTrayIcon));
        return Task.CompletedTask;
    }

    public Task HideTrayIcon()
    {
        MessageBox.Show(nameof(HideTrayIcon));
        return Task.CompletedTask;
    }

    public Task SendText(string text)
    {
        MessageBox.Show(text);
        return Task.CompletedTask;
    }
    
    public async Task<int> GetAnswer(string question)
    {
        return await Simulator.CalculateAnswer(question);
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
await client.ShowTrayIcon();
await client.SendText("Hello world!");
var result = await client.GetAnswer("What's the answer to the ultimate question of life, the universe, and everything?");
```

### Notes
The generated code currently requires C# version 8 and above. You can enable this using the following code in your .csproj file:
```xml
<PropertyGroup>
  <LangVersion>latest</LangVersion> <!-- or just 8.0 -->
</PropertyGroup>
```

### Contacts
* [mail](mailto:havendv@gmail.com)

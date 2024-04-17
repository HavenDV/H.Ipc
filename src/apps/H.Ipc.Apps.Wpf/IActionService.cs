using System.Windows;

namespace H.Ipc.Apps.Wpf;

public class Person
{
    public string Name { get; set; }
}

public interface IActionService
{
    Task SendText(string text);
    Task ShowTrayIcon();
    Task HideTrayIcon();

    Task<int> CalculateResult();
    Task<Person> GetPerson();

    //event EventHandler<string> TextReceived;
}

[H.IpcGenerators.IpcClient]
public partial class ActionServiceClient : IActionService
{
}

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

    public Task<Person> GetPerson()
    {
        return Task.FromResult(new Person());
    }

    public Task<int> CalculateResult()
    {
        return Task.FromResult(10);
    }

    public Task SendText(string text)
    {
        MessageBox.Show(text);
        return Task.CompletedTask;
    }
}

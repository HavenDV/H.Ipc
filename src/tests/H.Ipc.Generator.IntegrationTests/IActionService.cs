namespace H.Ipc.Generator.IntegrationTests;

public interface IActionService
{
    void ShowTrayIcon();
    void HideTrayIcon();
}

[H.IpcGenerators.IpcClient]
public partial class ActionServiceClient : IActionService
{
}

[H.IpcGenerators.IpcServer]
public partial class ActionService : IActionService
{
    public void ShowTrayIcon()
    {
    }

    public void HideTrayIcon()
    {
    }
}

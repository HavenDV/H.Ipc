namespace H.Ipc.Generator.IntegrationTests;

public interface IActionService
{
    void ShowTrayIcon();
    void HideTrayIcon();
    void SendText(string text);
}

[H.IpcGenerators.IpcClient]
public partial class ActionServiceClient : IActionService
{
}

[H.IpcGenerators.IpcServer]
public partial class ActionService : IActionService
{
    public bool ShowTrayIconResult { get; set; }
    public bool HideTrayIconResult { get; set; }
    public string SendTextResult { get; set; } = string.Empty;

    public void ShowTrayIcon()
    {
        ShowTrayIconResult = true;
    }

    public void HideTrayIcon()
    {
        HideTrayIconResult = true;
    }

    public void SendText(string text)
    {
        SendTextResult = text;
    }
}

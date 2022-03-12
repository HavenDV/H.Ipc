using System.Windows;

namespace H.Ipc.Apps.Wpf;

public interface IActionService
{
    void SendText(string text);
    void ShowTrayIcon();
    void HideTrayIcon();

    //event EventHandler<string> TextReceived;
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

namespace NamedPipesSample.Common
{
    public interface IActionService
    {
        //void SendText(string text);
        void ShowTrayIcon();
        void HideTrayIcon();

        //event EventHandler<string> TextReceived;
    }

    [H.IpcGenerators.IpcService]
    public partial class ActionServiceClient : IActionService
    {
    }
}

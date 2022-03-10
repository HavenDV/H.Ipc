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
    public partial class ActionServiceClient
    {
        public partial Task ShowTrayIconAsync(CancellationToken cancellationToken = default);

        public partial Task HideTrayIconAsync(CancellationToken cancellationToken = default);
    }
}

//HintName: IActionService.ServerRequests.generated.cs

#nullable enable

namespace H.Ipc.Apps.Wpf
{
    public class ShowTrayIconServerMethod : global::H.IpcGenerators.RunMethodRequest
    {
        public ShowTrayIconServerMethod()
        {
            Name = "ShowTrayIcon";
        }
    }

    public class HideTrayIconServerMethod : global::H.IpcGenerators.RunMethodRequest
    {
        public HideTrayIconServerMethod()
        {
            Name = "HideTrayIcon";
        }
    }

    public class SendTextServerMethod : global::H.IpcGenerators.RunMethodRequest
    {
        public string Text { get; set; }
        public SendTextServerMethod(string text)
        {
            Name = "SendText";
            Text = text ?? throw new global::System.ArgumentNullException(nameof(text));
        }
    }
}
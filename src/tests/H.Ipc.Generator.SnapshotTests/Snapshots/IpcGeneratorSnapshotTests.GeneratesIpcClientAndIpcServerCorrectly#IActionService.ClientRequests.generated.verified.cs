//HintName: IActionService.ClientRequests.generated.cs

#nullable enable

namespace H.Ipc.Apps.Wpf
{
    public class ShowTrayIconClientMethod : global::H.IpcGenerators.RunMethodRequest
    {
        public ShowTrayIconClientMethod()
        {
            Name = "ShowTrayIcon";
        }
    }

    public class HideTrayIconClientMethod : global::H.IpcGenerators.RunMethodRequest
    {
        public HideTrayIconClientMethod()
        {
            Name = "HideTrayIcon";
        }
    }

    public class SendTextClientMethod : global::H.IpcGenerators.RunMethodRequest
    {
        public string Text { get; set; }
        public SendTextClientMethod(string text)
        {
            Name = "SendText";
            Text = text ?? throw new global::System.ArgumentNullException(nameof(text));
        }
    }
}
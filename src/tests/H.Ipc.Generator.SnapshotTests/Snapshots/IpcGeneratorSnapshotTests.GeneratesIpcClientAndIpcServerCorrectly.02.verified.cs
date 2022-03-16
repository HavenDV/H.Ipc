//HintName: IActionService_Requests.generated.cs

#nullable enable

namespace H.Ipc.Apps.Wpf
{
    public class ShowTrayIconMethod : global::H.IpcGenerators.RunMethodRequest
    {


        public ShowTrayIconMethod()
        {
            Name = "ShowTrayIcon";

        }
    }

    public class HideTrayIconMethod : global::H.IpcGenerators.RunMethodRequest
    {


        public HideTrayIconMethod()
        {
            Name = "HideTrayIcon";

        }
    }

    public class SendTextMethod : global::H.IpcGenerators.RunMethodRequest
    {
        public string Text { get; set; }

        public SendTextMethod(string text)
        {
            Name = "SendText";
            Text = text ?? throw new global::System.ArgumentNullException(nameof(text));
        }
    }

}
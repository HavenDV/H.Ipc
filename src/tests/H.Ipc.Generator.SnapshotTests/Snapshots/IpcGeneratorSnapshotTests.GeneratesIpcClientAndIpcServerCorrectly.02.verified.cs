//HintName: IActionService_Requests.generated.cs

using System;
using H.IpcGenerators;

#nullable enable

namespace H.Ipc.Apps.Wpf
{
    public class ShowTrayIconMethod : RunMethodRequest
    {


        public ShowTrayIconMethod()
        {
            Name = "ShowTrayIcon";

        }
    }

    public class HideTrayIconMethod : RunMethodRequest
    {


        public HideTrayIconMethod()
        {
            Name = "HideTrayIcon";

        }
    }

    public class SendTextMethod : RunMethodRequest
    {
        public string Text { get; set; }

        public SendTextMethod(string text)
        {
            Name = "SendText";
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }
    }

}
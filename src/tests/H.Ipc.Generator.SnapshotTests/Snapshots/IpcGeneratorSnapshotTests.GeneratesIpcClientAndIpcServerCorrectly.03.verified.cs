//HintName: ActionService.generated.cs

using System;
using H.Pipes;

#nullable enable

namespace H.Ipc.Apps.Wpf
{
    public partial class ActionService
    {
        public void Initialize(PipeServer<string> pipeServer)
        {
            pipeServer = pipeServer ?? throw new ArgumentNullException(nameof(pipeServer));
            pipeServer.MessageReceived += (_, args) =>
            {
                var methodName = args.Message;
                switch (methodName)
                {

                    case nameof(ShowTrayIcon):
                        ShowTrayIcon();
                        break;

                    case nameof(HideTrayIcon):
                        HideTrayIcon();
                        break;

                }
            };
        }
    }
}
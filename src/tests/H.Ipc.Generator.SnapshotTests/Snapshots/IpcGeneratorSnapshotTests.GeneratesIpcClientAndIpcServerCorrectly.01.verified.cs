//HintName: ActionService.generated.cs

using System;
using H.Pipes;
using System.Text.Json;
using H.IpcGenerators;

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
                var json = args.Message ?? throw new InvalidOperationException("Message is null.");
                var request = Deserialize<RpcRequest>(json);

                if (request.Type == RpcRequestType.RunMethod)
                {
                    var method = Deserialize<RunMethodRequest>(json);
                    switch (method.Name)
                    {
                        case nameof(ShowTrayIcon):
                            {
                                var arguments = Deserialize<ShowTrayIconMethod>(json);
                                ShowTrayIcon();
                                break;
                            }

                        case nameof(HideTrayIcon):
                            {
                                var arguments = Deserialize<HideTrayIconMethod>(json);
                                HideTrayIcon();
                                break;
                            }

                        case nameof(SendText):
                            {
                                var arguments = Deserialize<SendTextMethod>(json);
                                SendText(arguments.Text);
                                break;
                            }
                    }
                }
            };
        }

        private static T Deserialize<T>(string json)
        {
            return
                JsonSerializer.Deserialize<T>(json) ??
                throw new ArgumentException($@"Returned null when trying to deserialize to {typeof(T)}.
    json:
    {json}");
        }
    }
}
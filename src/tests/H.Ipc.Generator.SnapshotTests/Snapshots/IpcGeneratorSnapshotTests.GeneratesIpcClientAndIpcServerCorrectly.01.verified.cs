//HintName: ActionService.generated.cs

#nullable enable

namespace H.Ipc.Apps.Wpf
{
    public partial class ActionService
    {
        public void Initialize(global::H.Pipes.IPipeConnection<string> connection)
        {
            connection = connection ?? throw new global::System.ArgumentNullException(nameof(connection));
            connection.MessageReceived += (_, args) =>
            {
                var json = args.Message ?? throw new global::System.InvalidOperationException("Message is null.");
                var request = Deserialize<global::H.IpcGenerators.RpcRequest>(json);

                if (request.Type == global::H.IpcGenerators.RpcRequestType.RunMethod)
                {
                    var method = Deserialize<global::H.IpcGenerators.RunMethodRequest>(json);
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
                global::System.Text.Json.JsonSerializer.Deserialize<T>(json) ??
                throw new global::System.ArgumentException($@"Returned null when trying to deserialize to {typeof(T)}.
    json:
    {json}");
        }
    }
}
//HintName: ActionService.IpcServer.generated.cs

#nullable enable

using H.IpcGenerators;

namespace H.Ipc.Apps.Wpf
{
    public partial class ActionService
    {
        #region Events

        public event global::System.EventHandler<global::System.Exception>? ExceptionOccurred;

        private void OnExceptionOccurred(global::System.Exception exception)
        {
            ExceptionOccurred?.Invoke(this, exception);
        }

        #endregion

        public void Initialize(global::H.Pipes.IPipeConnection<string> connection)
        {
            connection = connection ?? throw new global::System.ArgumentNullException(nameof(connection));
            connection.MessageReceived += async (_, args) =>
            {
                try
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
                                var arguments = Deserialize<ShowTrayIconServerMethod>(json);
                                await ShowTrayIcon();
                                break;
                            }
                            case nameof(HideTrayIcon):
                            {
                                var arguments = Deserialize<HideTrayIconServerMethod>(json);
                                await HideTrayIcon();
                                break;
                            }
                            case nameof(SendText):
                            {
                                var arguments = Deserialize<SendTextServerMethod>(json);
                                await SendText(arguments.Text);
                                break;
                            }
                            case nameof(GetPoints):
                            {
                                var arguments = Deserialize<GetPointsServerMethod>(json);
                                var resultCore = await GetPoints();
                                var result = ReturnMethodResultFactory.Create(resultCore);
                                var jsonStr = Serialize(result);
                                await connection.WriteAsync(jsonStr).ConfigureAwait(false);
                                break;
                            }
                        }
                    }
                }
                catch (global::System.Exception exception)
                {
                    OnExceptionOccurred(exception);
                    var result = new ReturnMethodResultRequest(false, exception.Message);
                    var jsonStr = Serialize(result);
                    await connection.WriteAsync(jsonStr).ConfigureAwait(false);
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

        private static string Serialize<T>(T obj)
        {
            return global::System.Text.Json.JsonSerializer.Serialize(obj);
        }
    }
}
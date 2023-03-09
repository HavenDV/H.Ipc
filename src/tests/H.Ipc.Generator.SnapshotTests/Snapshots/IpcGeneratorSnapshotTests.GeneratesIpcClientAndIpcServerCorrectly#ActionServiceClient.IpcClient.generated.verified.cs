//HintName: ActionServiceClient.IpcClient.generated.cs

#nullable enable

namespace H.Ipc.Apps.Wpf
{
    public partial class ActionServiceClient
    {
        #region Properties

        private global::H.Pipes.IPipeConnection<string>? Connection { get; set; }

        #endregion

        #region Events

        public event global::System.EventHandler<global::System.Exception>? ExceptionOccurred;

        private void OnExceptionOccurred(global::System.Exception exception)
        {
            ExceptionOccurred?.Invoke(this, exception);
        }

        #endregion

        public void Initialize(global::H.Pipes.IPipeConnection<string> connection)
        {
            Connection = connection ?? throw new global::System.ArgumentNullException(nameof(connection));
        }

        public async void ShowTrayIcon()
        {
            try
            {
                await WriteAsync(new ShowTrayIconClientMethod()).ConfigureAwait(false);
            }
            catch (global::System.Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        public async void HideTrayIcon()
        {
            try
            {
                await WriteAsync(new HideTrayIconClientMethod()).ConfigureAwait(false);
            }
            catch (global::System.Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        public async void SendText(string text)
        {
            try
            {
                await WriteAsync(new SendTextClientMethod(text)).ConfigureAwait(false);
            }
            catch (global::System.Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        private async global::System.Threading.Tasks.Task WriteAsync<T>(
            T method,
            global::System.Threading.CancellationToken cancellationToken = default)
            where T : global::H.IpcGenerators.RpcRequest
        {
            if (Connection == null)
            {
                throw new global::System.InvalidOperationException("You need to call Initialize() first.");
            }

            var json = global::System.Text.Json.JsonSerializer.Serialize(method);

            await Connection.WriteAsync(json, cancellationToken).ConfigureAwait(false);
        }
    }
}
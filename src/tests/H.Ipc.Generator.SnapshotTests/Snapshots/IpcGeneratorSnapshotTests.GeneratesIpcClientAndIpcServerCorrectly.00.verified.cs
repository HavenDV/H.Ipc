//HintName: ActionServiceClient.generated.cs

using System.Threading.Tasks;

#nullable enable

namespace H.Ipc.Apps.Wpf
{
    public partial class ActionServiceClient
    {
        private global::H.Pipes.IPipeConnection<string>? Connection { get; set; }

        public void Initialize(global::H.Pipes.IPipeConnection<string> connection)
        {
            Connection = connection ?? throw new global::System.ArgumentNullException(nameof(connection));
        }

        public async void ShowTrayIcon()
        {
            await WriteAsync(new ShowTrayIconMethod()).ConfigureAwait(false);
        }

        public async void HideTrayIcon()
        {
            await WriteAsync(new HideTrayIconMethod()).ConfigureAwait(false);
        }

        public async void SendText(string text)
        {
            await WriteAsync(new SendTextMethod(text)).ConfigureAwait(false);
        }

        private async Task WriteAsync<T>(
            T method,
            global::System.Threading.CancellationToken cancellationToken = default)
            where T : global::H.IpcGenerators.RpcRequest
        {
            if (Connection == null)
            {
                return;
            }

            var json = global::System.Text.Json.JsonSerializer.Serialize(method);

            await Connection.WriteAsync(json, cancellationToken).ConfigureAwait(false);
        }
    }
}
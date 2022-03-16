//HintName: ActionServiceClient.generated.cs

using System;
using H.Pipes;
using System.Text.Json;
using H.IpcGenerators;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace H.Ipc.Apps.Wpf
{
    public partial class ActionServiceClient
    {
        private IPipeConnection<string>? Connection { get; set; }

        public void Initialize(IPipeConnection<string> connection)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
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

        private async Task WriteAsync<T>(T method, CancellationToken cancellationToken = default)
            where T : RpcRequest
        {
            if (Connection == null)
            {
                return;
            }

            var json = JsonSerializer.Serialize(method);

            await Connection.WriteAsync(json, cancellationToken).ConfigureAwait(false);
        }
    }
}
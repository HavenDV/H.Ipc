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
        private PipeClient<string>? Client { get; set; }

        public void Initialize(PipeClient<string> pipeClient)
        {
            Client = pipeClient ?? throw new ArgumentNullException(nameof(pipeClient));
        }

        public async void ShowTrayIcon()
        {
            await WriteAsync(new ShowTrayIconMethod()).ConfigureAwait(false);
        }

        public async void HideTrayIcon()
        {
            await WriteAsync(new HideTrayIconMethod()).ConfigureAwait(false);
        }

        private async Task WriteAsync<T>(T method, CancellationToken cancellationToken = default)
            where T : RpcMethod
        {
            if (Client == null)
            {
                return;
            }

            var json = JsonSerializer.Serialize(method);

            await Client.WriteAsync(json, cancellationToken).ConfigureAwait(false);
        }
    }

    public class ShowTrayIconMethod : RpcMethod
    {


        public ShowTrayIconMethod()
        {
            Name = "ShowTrayIcon";

        }
    }

    public class HideTrayIconMethod : RpcMethod
    {


        public HideTrayIconMethod()
        {
            Name = "HideTrayIcon";

        }
    }

}
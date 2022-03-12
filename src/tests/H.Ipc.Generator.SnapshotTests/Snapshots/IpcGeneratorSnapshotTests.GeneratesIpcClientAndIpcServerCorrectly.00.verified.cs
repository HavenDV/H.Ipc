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

        public async void SendText(string text)
        {
            await WriteAsync(new SendTextMethod(text)).ConfigureAwait(false);
        }

        private async Task WriteAsync<T>(T method, CancellationToken cancellationToken = default)
            where T : RpcRequest
        {
            if (Client == null)
            {
                return;
            }

            var json = JsonSerializer.Serialize(method);

            await Client.WriteAsync(json, cancellationToken).ConfigureAwait(false);
        }
    }

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
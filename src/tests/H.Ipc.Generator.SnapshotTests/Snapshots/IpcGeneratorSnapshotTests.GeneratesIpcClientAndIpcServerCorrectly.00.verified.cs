//HintName: ActionServiceClient.generated.cs

using System;
using H.Pipes;

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
            if (Client == null)
            {
                return;
            }

            await Client.WriteAsync("ShowTrayIcon").ConfigureAwait(false);
        }

        public async void HideTrayIcon()
        {
            if (Client == null)
            {
                return;
            }

            await Client.WriteAsync("HideTrayIcon").ConfigureAwait(false);
        }

    }
}
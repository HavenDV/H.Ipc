//HintName: ActionServiceClient.IpcClient.generated.cs

#nullable enable

using H.IpcGenerators;
using System;
using System.Threading.Tasks;

namespace H.Ipc.Apps.Wpf
{
    public partial class ActionServiceClient
    {
        private bool isWaitingForServerResponse = false;

        #region Properties

        private global::H.Pipes.IPipeConnection<string>? _connection;
        private global::H.Pipes.IPipeConnection<string> Connection
        {
            get
            {
                return _connection ?? throw new global::System.InvalidOperationException("You need to call Initialize() first."); 
            }
        }

        #endregion

        #region Events

        public event global::System.EventHandler<global::System.Exception>? ExceptionOccurred;

        private void OnExceptionOccurred(global::System.Exception exception)
        {
            ExceptionOccurred?.Invoke(this, exception);
        }

        #endregion


        private void ThrowIfWaitingForServer()
        {
            if (isWaitingForServerResponse)
            {
                throw new InvalidOperationException("Cannot perform this operation while waiting for server response.");
            }
        }


        public void Initialize(global::H.Pipes.IPipeConnection<string>? connection)
        {
            _connection = connection ?? throw new global::System.ArgumentNullException(nameof(connection));
        }

        public async System.Threading.Tasks.Task ShowTrayIcon()
        {
            ThrowIfWaitingForServer();
            try
            {
                await WriteAsync(new ShowTrayIconClientMethod()).ConfigureAwait(false);
            }
            catch (global::System.Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        public async System.Threading.Tasks.Task HideTrayIcon()
        {
            ThrowIfWaitingForServer();
            try
            {
                await WriteAsync(new HideTrayIconClientMethod()).ConfigureAwait(false);
            }
            catch (global::System.Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        public async System.Threading.Tasks.Task SendText(string text)
        {
            ThrowIfWaitingForServer();
            try
            {
                await WriteAsync(new SendTextClientMethod(text)).ConfigureAwait(false);
            }
            catch (global::System.Exception exception)
            {
                OnExceptionOccurred(exception);
            }
        }

        public async System.Threading.Tasks.Task<int> GetPoints()
        {
            ThrowIfWaitingForServer();

            var tcs = new TaskCompletionSource<Int32>();

            void ReceiveResult(object? sender, H.Pipes.Args.ConnectionMessageEventArgs<string?> e)
            {
                var jsonResult = e.Message ?? throw new ArgumentException("Message property of received H.Pipes.Args.ConnectionMessageEventArgs<string> object is null");
                var result = default(Int32);
                var resultGeneral = global::System.Text.Json.JsonSerializer.Deserialize<ReturnMethodResultRequest>(jsonResult);
                if (resultGeneral?.ResultType == "Int32")
                {
                    var resultSpecific = global::System.Text.Json.JsonSerializer.Deserialize<ReturnMethodResultRequest<Int32>>(jsonResult);
                    if (resultSpecific != null)
                    {
                        result = resultSpecific.Result;
                    }
                }

                Connection.MessageReceived -= ReceiveResult;
                tcs.SetResult(result);
            }

            isWaitingForServerResponse = true;
            try
            {
                Connection.MessageReceived += ReceiveResult;
                await WriteAsync(new GetPointsClientMethod()).ConfigureAwait(false);

                var result = await tcs.Task;
                isWaitingForServerResponse = false;
                return result;
            }
            catch (global::System.Exception exception)
            {
                isWaitingForServerResponse = false;
                OnExceptionOccurred(exception);
                return await Task.FromException<int>(exception);
            }
        }

        private async global::System.Threading.Tasks.Task WriteAsync<T>(
            T method,
            global::System.Threading.CancellationToken cancellationToken = default)
            where T : global::H.IpcGenerators.RpcRequest
        {
            var json = global::System.Text.Json.JsonSerializer.Serialize(method);
            await Connection.WriteAsync(json, cancellationToken).ConfigureAwait(false);
        }
    }
}
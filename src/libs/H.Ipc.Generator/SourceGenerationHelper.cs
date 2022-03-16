namespace H.Ipc.Generator;

internal class SourceGenerationHelper
{
    public static string GenerateClientImplementation(ClassData @class)
    {
        return @$"
#nullable enable

namespace {@class.Namespace}
{{
    public partial class {@class.Name}
    {{
        #region Properties

        private global::H.Pipes.IPipeConnection<string>? Connection {{ get; set; }}

        #endregion

        #region Events

{GenerateExceptionOccurredEvent()}

        #endregion

        public void Initialize(global::H.Pipes.IPipeConnection<string> connection)
        {{
            Connection = connection ?? throw new global::System.ArgumentNullException(nameof(connection));
        }}

{@class.Methods.Select(static method => $@"
        public async {method.ReturnType} {method.Name}({string.Join(", ", method.Parameters.Select(static parameter => $"{parameter.Type} {parameter.Name}"))})
        {{
            try
            {{
                await WriteAsync(new {method.Name}Method({string.Join(", ", method.Parameters.Select(static parameter => parameter.Name))})).ConfigureAwait(false);
            }}
            catch (global::System.Exception exception)
            {{
                OnExceptionOccurred(exception);
            }}
        }}
").Inject()}

        private async global::System.Threading.Tasks.Task WriteAsync<T>(
            T method,
            global::System.Threading.CancellationToken cancellationToken = default)
            where T : global::H.IpcGenerators.RpcRequest
        {{
            if (Connection == null)
            {{
                throw new global::System.InvalidOperationException(""You need to call Initialize() first."");
            }}

            var json = global::System.Text.Json.JsonSerializer.Serialize(method);

            await Connection.WriteAsync(json, cancellationToken).ConfigureAwait(false);
        }}
    }}
}}";
    }

    public static string GenerateServerImplementation(ClassData @class)
    {
        return @$"
#nullable enable

namespace {@class.Namespace}
{{
    public partial class {@class.Name}
    {{
        #region Events

{GenerateExceptionOccurredEvent()}

        #endregion

        public void Initialize(global::H.Pipes.IPipeConnection<string> connection)
        {{
            connection = connection ?? throw new global::System.ArgumentNullException(nameof(connection));
            connection.MessageReceived += (_, args) =>
            {{
                try
                {{
                    var json = args.Message ?? throw new global::System.InvalidOperationException(""Message is null."");
                    var request = Deserialize<global::H.IpcGenerators.RpcRequest>(json);

                    if (request.Type == global::H.IpcGenerators.RpcRequestType.RunMethod)
                    {{
                        var method = Deserialize<global::H.IpcGenerators.RunMethodRequest>(json);
                        switch (method.Name)
                        {{
{@class.Methods.Select(static method => $@"
                            case nameof({method.Name}):
                                {{
                                    var arguments = Deserialize<{method.Name}Method>(json);
                                    {method.Name}({string.Join(", ", method.Parameters.Select(static parameter => $"arguments.{parameter.Name.ToPropertyName()}")).TrimEnd(',', ' ', '\n', '\r')});
                                    break;
                                }}
").Inject()}
                        }}
                    }}
                }}
                catch (global::System.Exception exception)
                {{
                    OnExceptionOccurred(exception);
                }}
            }};
        }}

        private static T Deserialize<T>(string json)
        {{
            return
                global::System.Text.Json.JsonSerializer.Deserialize<T>(json) ??
                throw new global::System.ArgumentException($@""Returned null when trying to deserialize to {{typeof(T)}}.
    json:
    {{json}}"");
        }}
    }}
}}";
    }

    public static string GenerateRequests(ClassData @class)
    {
        return @$"
#nullable enable

namespace {@class.Namespace}
{{
{@class.Methods.Select(static method => $@"
    public class {method.Name}Method : global::H.IpcGenerators.RunMethodRequest
    {{
{method.Parameters.Select(static parameter => $@"
        public {parameter.Type} {parameter.Name.ToPropertyName()} {{ get; set; }}
").Inject()}

        public {method.Name}Method({string.Join(", ", method.Parameters.Select(static parameter => $"{parameter.Type} {parameter.Name}"))})
        {{
            Name = ""{method.Name}"";
{method.Parameters.Select(static parameter => $@"
            {parameter.Name.ToPropertyName()} = {parameter.Name} ?? throw new global::System.ArgumentNullException(nameof({parameter.Name}));
").Inject()}
        }}
    }}
").Inject()}

}}";
    }

    public static string GenerateExceptionOccurredEvent()
    {
        return @$"
        public event global::System.EventHandler<global::System.Exception>? ExceptionOccurred;

        private void OnExceptionOccurred(global::System.Exception exception)
        {{
            ExceptionOccurred?.Invoke(this, exception);
        }}
";
    }
}

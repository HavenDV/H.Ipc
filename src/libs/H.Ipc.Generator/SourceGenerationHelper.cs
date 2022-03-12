namespace H.Ipc.Generator;

internal class SourceGenerationHelper
{
    public static string GenerateClientImplementation(ClassData @class)
    {
        return @$"
using System;
using H.Pipes;
using System.Text.Json;
using H.IpcGenerators;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace {@class.Namespace}
{{
    public partial class {@class.Name}
    {{
        private PipeClient<string>? Client {{ get; set; }}

        public void Initialize(PipeClient<string> pipeClient)
        {{
            Client = pipeClient ?? throw new ArgumentNullException(nameof(pipeClient));
        }}

{@class.Methods.Select(static method => $@"
        public async {method.ReturnType} {method.Name}({string.Join(", ", method.Parameters.Select(static parameter => $"{parameter.Type} {parameter.Name}"))})
        {{
            await WriteAsync(new {method.Name}Method({string.Join(", ", method.Parameters.Select(static parameter => parameter.Name))})).ConfigureAwait(false);
        }}
").Inject()}

        private async Task WriteAsync<T>(T method, CancellationToken cancellationToken = default)
            where T : RpcRequest
        {{
            if (Client == null)
            {{
                return;
            }}

            var json = JsonSerializer.Serialize(method);

            await Client.WriteAsync(json, cancellationToken).ConfigureAwait(false);
        }}
    }}

{@class.Methods.Select(static method => $@"
    public class {method.Name}Method : RunMethodRequest
    {{
{method.Parameters.Select(static parameter => $@"
        public {parameter.Type} {parameter.Name.ToPropertyName()} {{ get; set; }}
").Inject()}

        public {method.Name}Method({string.Join(", ", method.Parameters.Select(static parameter => $"{parameter.Type} {parameter.Name}"))})
        {{
            Name = ""{method.Name}"";
{method.Parameters.Select(static parameter => $@"
            {parameter.Name.ToPropertyName()} = {parameter.Name} ?? throw new ArgumentNullException(nameof({parameter.Name}));
").Inject()}
        }}
    }}
").Inject()}

}}";
    }

    public static string GenerateServerImplementation(ClassData @class)
    {
        return @$"
using System;
using H.Pipes;
using System.Text.Json;
using H.IpcGenerators;

#nullable enable

namespace {@class.Namespace}
{{
    public partial class {@class.Name}
    {{
        public void Initialize(PipeServer<string> pipeServer)
        {{
            pipeServer = pipeServer ?? throw new ArgumentNullException(nameof(pipeServer));
            pipeServer.MessageReceived += (_, args) =>
            {{
                var json = args.Message ?? throw new InvalidOperationException(""Message is null."");
                var request = Deserialize<RpcRequest>(json);

                if (request.Type == RpcRequestType.RunMethod)
                {{
                    var method = Deserialize<RunMethodRequest>(json);
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
            }};
        }}

        private static T Deserialize<T>(string json)
        {{
            return
                JsonSerializer.Deserialize<T>(json) ??
                throw new ArgumentException($@""Returned null when trying to deserialize to {{typeof(T)}}.
    json:
    {{json}}"");
        }}
    }}
}}";
    }
}

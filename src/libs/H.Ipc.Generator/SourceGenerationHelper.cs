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

{string.Concat(@class.Methods.Select(static method => $@"
        public async {method.ReturnType} {method.Name}({string.Join(", ", method.Parameters.Select(static x => $"{x.Type} {x.Name}"))})
        {{
            await WriteAsync(new RpcMethod
            {{
                Name = ""{method.Name}"",
            }}).ConfigureAwait(false);
        }}
"))}

        private async Task WriteAsync(RpcMethod method, CancellationToken cancellationToken = default)
        {{
            if (Client == null)
            {{
                return;
            }}

            var json = JsonSerializer.Serialize(method);

            await Client.WriteAsync(json, cancellationToken).ConfigureAwait(false);
        }}
    }}
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
                var method = Deserialize<RpcMethod>(json);

                switch (method.Name)
                {{
{string.Concat(@class.Methods.Select(static method => $@"
                    case nameof({method.Name}):
                        {method.Name}();
                        break;
"))}
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

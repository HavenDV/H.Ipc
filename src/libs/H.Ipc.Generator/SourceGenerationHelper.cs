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
        public async {method.ReturnType} {method.Name}({string.Join(", ", method.Parameters.Select(static parameter => $"{parameter.Type} {parameter.Name}"))})
        {{
            await WriteAsync(new {method.Name}Method(
{string.Concat(method.Parameters.Select(static parameter => $@"
                {parameter.Name},
")).TrimEnd(',', ' ', '\n', '\r')}            
            )
            {{
                Name = ""{method.Name}"",
            }}).ConfigureAwait(false);
        }}
"))}

        private async Task WriteAsync<T>(T method, CancellationToken cancellationToken = default)
            where T : RpcMethod
        {{
            if (Client == null)
            {{
                return;
            }}

            var json = JsonSerializer.Serialize(method);

            await Client.WriteAsync(json, cancellationToken).ConfigureAwait(false);
        }}
    }}

{string.Concat(@class.Methods.Select(static method => $@"
    public class {method.Name}Method : RpcMethod
    {{
{string.Concat(method.Parameters.Select(static parameter => $@"
        public {parameter.Type} {parameter.Name.ToPropertyName()} {{ get; set; }}
"))}

        public {method.Name}Method(
{string.Concat(method.Parameters.Select(static parameter => $@"
            {parameter.Type} {parameter.Name},
")).TrimEnd(',', ' ', '\n', '\r')}
            )
        {{
{string.Concat(method.Parameters.Select(static parameter => $@"
            {parameter.Name.ToPropertyName()} = {parameter.Name} ?? throw new ArgumentNullException(nameof({parameter.Name}));
"))}
        }}
    }}
"))}

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
                    {{
                        var arguments = Deserialize<{method.Name}Method>(json);
                        {method.Name}(
{string.Concat(method.Parameters.Select(static parameter => $@"
                            arguments.{parameter.Name.ToPropertyName()},
")).TrimEnd(',', ' ', '\n', '\r')}
                            );
                        break;
                    }}
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

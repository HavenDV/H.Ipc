namespace H.Ipc.Generator;

internal class SourceGenerationHelper
{
    public static string GenerateClientImplementation(ClassData @class)
    {
        return @$"
using System;
using H.Pipes;

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
            if (Client == null)
            {{
                return;
            }}

            await Client.WriteAsync(""{method.Name}"").ConfigureAwait(false);
        }}
"))}
    }}
}}";
    }

    public static string GenerateServerImplementation(ClassData @class)
    {
        return @$"
using System;
using H.Pipes;

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
                var methodName = args.Message;
                switch (methodName)
                {{
{string.Concat(@class.Methods.Select(static method => $@"
                    case nameof({method.Name}):
                        {method.Name}();
                        break;
"))}
                }}
            }};
        }}
    }}
}}";
    }
}

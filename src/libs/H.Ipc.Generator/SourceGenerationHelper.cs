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
        public PipeClient<string>? Client {{ get; set; }}

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
}

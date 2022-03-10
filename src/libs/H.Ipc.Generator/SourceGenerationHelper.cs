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
        public partial async {method.ReturnType.GetText()} {method.Identifier.Text}{method.ParameterList}
        {{
            if (Client == null)
            {{
                return;
            }}

            await Client.WriteAsync(""{method.Identifier.Text}"").ConfigureAwait(false);
        }}
"))}
    }}
}}";
    }
}

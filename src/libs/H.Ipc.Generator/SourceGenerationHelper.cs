using System.Data;
using H.Generators.Extensions;
using Microsoft.CodeAnalysis;

namespace H.Generators;

internal static class SourceGenerationHelper
{
    public static string GenerateClientImplementation(ClassData @class)
    {
        return @$"
#nullable enable

using H.IpcGenerators;
using System;
using System.Threading.Tasks;

namespace {@class.Namespace}
{{
    public partial class {@class.Name}
    {{
        private bool isWaitingForServerResponse = false;

        #region Properties

        private global::H.Pipes.IPipeConnection<string>? _connection;
        private global::H.Pipes.IPipeConnection<string> Connection
        {{
            get
            {{
                return _connection ?? throw new global::System.InvalidOperationException(""You need to call Initialize() first.""); 
            }}
        }}

        #endregion

        #region Events

{GenerateExceptionOccurredEvent()}

        #endregion

{GenerateThrowIfWaitingMethod()}

        public void Initialize(global::H.Pipes.IPipeConnection<string>? connection)
        {{
            _connection = connection ?? throw new global::System.ArgumentNullException(nameof(connection));
        }}

{@class.Methods.Select(static method => GenerateClientMethod(method)).Inject()}

        private async global::System.Threading.Tasks.Task WriteAsync<T>(
            T method,
            global::System.Threading.CancellationToken cancellationToken = default)
            where T : global::H.IpcGenerators.RpcRequest
        {{
            var json = global::System.Text.Json.JsonSerializer.Serialize(method);
            await Connection.WriteAsync(json, cancellationToken).ConfigureAwait(false);
        }}
    }}
}}";
    }

    private static string GenerateThrowIfWaitingMethod()
    {
        return @"
        private void ThrowIfWaitingForServer()
        {
            if (isWaitingForServerResponse)
            {
                throw new InvalidOperationException(""Cannot perform this operation while waiting for server response."");
            }
        }
";
    }

    private static string GenerateClientMethod(IMethodSymbol method)
    {
        var isTask = method.ReturnType.Name == nameof(Task);
        var isInSystemNameSpace = method.ReturnType.ContainingNamespace.ToDisplayString() == typeof(Task).Namespace;        
        if (!(isTask && isInSystemNameSpace))
        {
            throw new InvalidExpressionException(
                $"Method '{method.Name}' in interface '{method.ContainingType.Name}' must declare return type 'Task' or 'Task<T>'. Is Task: {isTask}  Is in System namespace: {isInSystemNameSpace}");
        }

        if(method.ReturnType is INamedTypeSymbol { Arity: 1 } namedTypeSymbol)
        {
            var coreReturnType = namedTypeSymbol.TypeArguments.First();
            
            return $@"
        public async {method.ReturnType} {method.Name}({string.Join(", ", method.Parameters.Select(static parameter => $"{parameter.Type} {parameter.Name}"))})
        {{
            ThrowIfWaitingForServer();

            var tcs = new TaskCompletionSource<{coreReturnType.Name}>();

            void ReceiveResult(object? sender, H.Pipes.Args.ConnectionMessageEventArgs<string?> e)
            {{
                var jsonResult = e.Message ?? throw new ArgumentException(""Message property of received H.Pipes.Args.ConnectionMessageEventArgs<string> object is null"");
                var result = default({coreReturnType.Name});
                var resultGeneral = global::System.Text.Json.JsonSerializer.Deserialize<ReturnMethodResultRequest>(jsonResult);
                if (resultGeneral?.ResultType == ""{coreReturnType.Name}"")
                {{
                    var resultSpecific = global::System.Text.Json.JsonSerializer.Deserialize<ReturnMethodResultRequest<{coreReturnType.Name}>>(jsonResult);
                    if (resultSpecific != null)
                    {{
                        result = resultSpecific.Result;
                    }}
                }}

                Connection.MessageReceived -= ReceiveResult;
                {GenerateTaskCompletionAssignment(coreReturnType)}
            }}

            isWaitingForServerResponse = true;
            try
            {{
                Connection.MessageReceived += ReceiveResult;
                await WriteAsync(new {method.Name}ClientMethod({string.Join(", ", method.Parameters.Select(static parameter => parameter.Name))})).ConfigureAwait(false);

                var result = await tcs.Task;
                isWaitingForServerResponse = false;
                return result;
            }}
            catch (global::System.Exception exception)
            {{
                isWaitingForServerResponse = false;
                OnExceptionOccurred(exception);
                return await Task.FromException<{coreReturnType}>(exception);
            }}
        }}
";
        }
        else
        {
            return $@"
        public async {method.ReturnType} {method.Name}({string.Join(", ", method.Parameters.Select(static parameter => $"{parameter.Type} {parameter.Name}"))})
        {{
            ThrowIfWaitingForServer();
            try
            {{
                await WriteAsync(new {method.Name}ClientMethod({string.Join(", ", method.Parameters.Select(static parameter => parameter.Name))})).ConfigureAwait(false);
            }}
            catch (global::System.Exception exception)
            {{
                OnExceptionOccurred(exception);
            }}
        }}
";
        }
    }

    private static string GenerateTaskCompletionAssignment(ITypeSymbol coreReturnType)
    {
        if (coreReturnType.IsReferenceType)
        {
            return $@"
                if (result is null)
                {{
                    tcs.SetException(new global::System.Text.Json.JsonException(""Failed to deserialize JSON result""));
                }}
                else
                {{
                    tcs.SetResult(result);
                }}
";
        }
        else
        {
            return $@"tcs.SetResult(result);";
        }
    }

    public static string GenerateServerImplementation(ClassData @class)
    {
        return @$"
#nullable enable

using H.IpcGenerators;

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
            connection.MessageReceived += async (_, args) =>
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
{@class.Methods.Select(static method => GenerateServerUnpackMethod(method)).Inject()}
                        }}
                    }}
                }}
                catch (global::System.Exception exception)
                {{
                    OnExceptionOccurred(exception);
                    var result = new ReturnMethodResultRequest(false, exception.Message);
                    var jsonStr = Serialize(result);
                    await connection.WriteAsync(jsonStr).ConfigureAwait(false);
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

        private static string Serialize<T>(T obj)
        {{
            return global::System.Text.Json.JsonSerializer.Serialize(obj);
        }}
    }}
}}";
    }

    private static string GenerateServerUnpackMethod(IMethodSymbol method)
    {
        var isTask = method.ReturnType.Name == nameof(Task);
        var isInSystemNameSpace = method.ReturnType.ContainingNamespace.ToDisplayString() == typeof(Task).Namespace;        
        if (!(isTask && isInSystemNameSpace))
        {
            throw new InvalidExpressionException(
                $"Method '{method.Name}' in interface '{method.ContainingType.Name}' must declare return type 'Task' or 'Task<T>'.");
        }

        if (method.ReturnType is INamedTypeSymbol { Arity: 1 } namedTypeSymbol)
        {
            return $@"
                            case nameof({method.Name}):
                            {{
                                var arguments = Deserialize<{method.Name}ServerMethod>(json);
                                var resultCore = await {method.Name}({string.Join(", ", method.Parameters.Select(static parameter => $"arguments.{parameter.Name.ToPropertyName()}")).TrimEnd(',', ' ', '\n', '\r')});
                                var result = ReturnMethodResultFactory.Create(resultCore);
                                var jsonStr = Serialize(result);
                                await connection.WriteAsync(jsonStr).ConfigureAwait(false);
                                break;
                            }}";
        }
        else
        {
            return $@"
                            case nameof({method.Name}):
                            {{
                                var arguments = Deserialize<{method.Name}ServerMethod>(json);
                                await {method.Name}({string.Join(", ", method.Parameters.Select(static parameter => $"arguments.{parameter.Name.ToPropertyName()}")).TrimEnd(',', ' ', '\n', '\r')});
                                break;
                            }}";
        }
    }

    public static string GenerateRequests(ClassData @class, bool server)
    {
        return @$"
#nullable enable

namespace {@class.Namespace}
{{
{@class.Methods.Select(method => $@"
    public class {method.Name}{(server ? "Server" : "Client")}Method : global::H.IpcGenerators.RunMethodRequest
    {{
{method.Parameters.Select(static parameter => $@"
        public {parameter.Type} {parameter.Name.ToPropertyName()} {{ get; set; }}
").Inject()}
 
        public {method.Name}{(server ? "Server" : "Client")}Method({string.Join(", ", method.Parameters.Select(static parameter => $"{parameter.Type} {parameter.Name}"))})
        {{
            Name = ""{method.Name}"";
{method.Parameters.Select(static parameter => $@" 
            {parameter.Name.ToPropertyName()} = {parameter.Name} ?? throw new global::System.ArgumentNullException(nameof({parameter.Name}));
 ").Inject()}
        }}
    }}
").Inject()}
}}".RemoveBlankLinesWhereOnlyWhitespaces();
    }

    public static string GenerateExceptionOccurredEvent()
    {
        return @" 
        public event global::System.EventHandler<global::System.Exception>? ExceptionOccurred;

        private void OnExceptionOccurred(global::System.Exception exception)
        {
            ExceptionOccurred?.Invoke(this, exception);
        }
 ".RemoveBlankLinesWhereOnlyWhitespaces();
    }
}

namespace H.IpcGenerators;

public class RunMethodRequest : RpcRequest
{
    public string Name { get; set; } = string.Empty;

    public RunMethodRequest() : base(RpcRequestType.RunMethod)
    {
    }
}

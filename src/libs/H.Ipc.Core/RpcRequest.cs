namespace H.IpcGenerators;

public class RpcRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public RpcRequestType Type { get; set; }

    public RpcRequest(RpcRequestType type)
    {
        Type = type;
    }
}

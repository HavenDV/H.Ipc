namespace H.IpcGenerators;

/// <summary>
/// 
/// </summary>
public class RpcRequest
{
    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 
    /// </summary>
    public RpcRequestType Type { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    public RpcRequest(RpcRequestType type)
    {
        Type = type;
    }
}

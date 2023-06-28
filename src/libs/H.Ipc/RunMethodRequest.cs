namespace H.IpcGenerators;

/// <summary>
/// 
/// </summary>
public class RunMethodRequest : RpcRequest
{
    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public RunMethodRequest() : base(RpcRequestType.RunMethod)
    {
    }
}

namespace H.IpcGenerators;

public class RpcMethod
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
}

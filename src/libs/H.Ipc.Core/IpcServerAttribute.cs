namespace H.IpcGenerators;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
[Conditional("IPCGENERATOR_ATTRIBUTES")]
public sealed class IpcServerAttribute : Attribute
{
}
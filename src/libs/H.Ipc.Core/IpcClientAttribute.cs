using System.Diagnostics;

namespace H.IpcGenerators;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
[Conditional("IPCGENERATOR_ATTRIBUTES")]
public sealed class IpcClientAttribute : Attribute
{
}
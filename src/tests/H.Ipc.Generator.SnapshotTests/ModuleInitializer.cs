using System.Runtime.CompilerServices;

namespace H.Ipc.Generator.IntegrationTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
    }
}
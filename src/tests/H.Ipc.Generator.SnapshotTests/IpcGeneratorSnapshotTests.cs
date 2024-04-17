namespace H.Generators.IntegrationTests;

[TestClass]
public class IpcGeneratorSnapshotTests : VerifyBase
{
    [TestMethod]
    public Task GeneratesIpcClientAndIpcServerCorrectly()
    {
        return this.CheckSourceAsync(@"
using System.Threading.Tasks;

namespace H.Ipc.Apps.Wpf;

public interface IActionService
{
    Task ShowTrayIcon();
    Task HideTrayIcon();
    Task SendText(string text);
    Task<int> GetPoints();
}

[H.IpcGenerators.IpcClient]
public partial class ActionServiceClient : IActionService
{
}

[H.IpcGenerators.IpcServer]
public partial class ActionService : IActionService
{
    public Task ShowTrayIcon()
    {
        return Task.CompletedTask;
    }

    public Task HideTrayIcon()
    {
        return Task.CompletedTask;
    }

    public Task SendText(string text)
    {
        return Task.CompletedTask;
    }

    public Task<int> GetPoints()
    {
        return Task.FromResult(42);
    }
}");
    }
}
namespace H.Generators.IntegrationTests;

[TestClass]
public class IpcGeneratorSnapshotTests : VerifyBase
{
    [TestMethod]
    public Task GeneratesIpcClientAndIpcServerCorrectly()
    {
        return this.CheckSourceAsync(@"
namespace H.Ipc.Apps.Wpf;

public interface IActionService
{
    void ShowTrayIcon();
    void HideTrayIcon();
    void SendText(string text);
}

[H.IpcGenerators.IpcClient]
public partial class ActionServiceClient : IActionService
{
}

[H.IpcGenerators.IpcServer]
public partial class ActionService : IActionService
{
    public void ShowTrayIcon()
    {
    }

    public void HideTrayIcon()
    {
    }

    public void SendText(string text)
    {
    }
}");
    }
}
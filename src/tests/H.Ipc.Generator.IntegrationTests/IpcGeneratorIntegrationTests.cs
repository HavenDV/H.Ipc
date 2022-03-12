using H.Pipes;

namespace H.Ipc.Generator.IntegrationTests;

[TestClass]
public class IpcGeneratorIntegrationTests
{
    [TestMethod]
    public async Task GeneratesIpcClientAndIpcServerCorrectly()
    {
        const string serverName = nameof(GeneratesIpcClientAndIpcServerCorrectly);

        // Server initialization
        await using var server = new PipeServer<string>(serverName);
        var service = new ActionService();
        service.Initialize(server);
        await server.StartAsync();

        // Client initialization
        await using var client = new PipeClient<string>(serverName);
        var serviceClient = new ActionServiceClient();
        serviceClient.Initialize(client);
        await client.ConnectAsync();

        // Client usage
        serviceClient.ShowTrayIcon();
        serviceClient.HideTrayIcon();
        serviceClient.SendText("Hello, World!");

        await Task.Delay(TimeSpan.FromSeconds(5));

        service.ShowTrayIconResult.Should().BeTrue();
        service.HideTrayIconResult.Should().BeTrue();
        service.SendTextResult.Should().Be("Hello, World!");
    }
}
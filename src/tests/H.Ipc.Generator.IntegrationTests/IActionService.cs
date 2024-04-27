namespace H.Ipc.Generator.IntegrationTests;

public class DataPoint
{
    public int X { get; set; }
    public int Y { get; set; }
    public float Temperature { get; set; }
}

public interface IActionService
{
    Task<DataPoint> GetMeasurement();
    Task<int> GetPoints();
    Task ShowTrayIcon();
    Task HideTrayIcon();
    Task SendText(string text);
}

[H.IpcGenerators.IpcClient]
public partial class ActionServiceClient : IActionService
{
}

[H.IpcGenerators.IpcServer]
public partial class ActionService : IActionService
{
    public bool ShowTrayIconResult { get; set; }
    public bool HideTrayIconResult { get; set; }
    public string SendTextResult { get; set; } = string.Empty;

    public Task ShowTrayIcon()
    {
        ShowTrayIconResult = true;
        return Task.CompletedTask;
    }

    public Task HideTrayIcon()
    {
        HideTrayIconResult = true;
        return Task.CompletedTask;
    }

    public Task SendText(string text)
    {
        SendTextResult = text;
        return Task.CompletedTask;
    }

    public Task<int> GetPoints()
    {
        return Task.FromResult(42);
    }

    public Task<DataPoint> GetMeasurement()
    {
        return Task.FromResult(new DataPoint { X = 10, Y = 20, Temperature = 25 });
    }
}

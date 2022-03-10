using H.Pipes;
using System.Windows;

namespace H.Ipc.Apps.Wpf;

public partial class ServerWindow
{
    #region Constants

    public const string ServerName = "H.Ipc.Apps.Wpf";

    #endregion

    #region Properties

    private PipeServer<string> Server { get; }
    private ActionService ActionService { get; set; } = new();

    #endregion

    #region Constructors

    public ServerWindow()
    {
        InitializeComponent();

        Server = new PipeServer<string>(ServerName);
        Server.ExceptionOccurred += (_, args) =>
        {
            WriteLine($"{nameof(Server.ExceptionOccurred)}: {args.Exception}");
        };
        Server.MessageReceived += (_, args) =>
        {
            WriteLine($"{nameof(Server.MessageReceived)}: {args.Message}");
        };
        Server.ClientConnected += (_, args) =>
        {
            WriteLine($"{nameof(Server.ClientConnected)}: {args.Connection.PipeName}");
        };
        ActionService.Initialize(Server);
    }

    #endregion

    #region Methods

    public void WriteLine(string text)
    {
        Dispatcher.Invoke(() =>
            ConsoleTextBox.Text += text + Environment.NewLine);
    }

    #endregion

    #region Event Handlers

    private async void Window_Loaded(object _, RoutedEventArgs e)
    {
        try
        {
            await Server.StartAsync().ConfigureAwait(false);

            WriteLine($"Started");
        }
        catch (Exception exception)
        {
            WriteLine($"{nameof(Window_Loaded)}: {exception}");
        }
    }

    private async void Window_Unloaded(object _, RoutedEventArgs e)
    {
        try
        {
            await Server.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            WriteLine($"{nameof(Window_Unloaded)}: {exception}");
        }
    }

    #endregion

}

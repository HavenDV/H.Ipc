using System.Diagnostics.CodeAnalysis;
using System.Windows;
using H.Pipes;

namespace H.Ipc.Apps.Wpf;

public partial class ClientWindow
{
    #region Properties

    private PipeClient<string> Client { get; }
    private ActionServiceClient ActionServiceClient { get; set; } = new();

    #endregion

    #region Constructors

    public ClientWindow()
    {
        InitializeComponent();

        Client = new PipeClient<string>(ServerWindow.ServerName);
        Client.ExceptionOccurred += (_, args) =>
        {
            WriteLine($"{nameof(Client.ExceptionOccurred)}: {args.Exception}");
        };
        Client.Connected += (_, args) =>
        {
            WriteLine($"{nameof(Client.Connected)}");
        };
        Client.Disconnected += (_, args) =>
        {
            WriteLine($"{nameof(Client.Disconnected)}");
        };
        ActionServiceClient.Initialize(Client);

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
            await Client.ConnectAsync().ConfigureAwait(false);
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
            await Client.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            WriteLine($"{nameof(Window_Unloaded)}: {exception}");
        }
    }

    private async void RaiseEvent1Button_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ActionServiceClient.ShowTrayIcon();
            //if (Instance == null)
            //{
            //    return;
            //}

            //Instance.RaiseEvent1();
            //WriteLine($"{nameof(Instance.RaiseEvent1)}");
        }
        catch (Exception exception)
        {
            WriteLine($"{nameof(RaiseEvent1Button_Click)}: {exception}");
        }
    }

    private async void RaiseEvent3Button_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ActionServiceClient.HideTrayIcon();
            //if (Instance == null)
            //{
            //    return;
            //}

            //Instance.RaiseEvent3();
            //WriteLine($"{nameof(Instance.RaiseEvent3)}");
        }
        catch (Exception exception)
        {
            WriteLine($"{nameof(RaiseEvent3Button_Click)}: {exception}");
        }
    }

    private void Method1Button_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            //if (Instance == null)
            //{
            //    return;
            //}

            //var result = Instance.Method1(123);
            //WriteLine($"{nameof(Instance.Method1)}: {result}");
        }
        catch (Exception exception)
        {
            WriteLine($"{nameof(Method1Button_Click)}: {exception}");
        }
    }

    private async void Method2Button_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ActionServiceClient.SendText(Method2ArgumentTextBox.Text);
        }
        catch (Exception exception)
        {
            WriteLine($"{nameof(Method2Button_Click)}: {exception}");
        }
    }

    #endregion
}

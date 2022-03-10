namespace H.Ipc.Apps.Wpf;

public partial class MainWindow
{
    #region Constructors

    public MainWindow()
    {
        InitializeComponent();
    }

    #endregion

    #region Event Handlers

    private void ClientButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        new ClientWindow().Show();
    }

    private void ServerButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        new ServerWindow().Show();
    }

    #endregion
}

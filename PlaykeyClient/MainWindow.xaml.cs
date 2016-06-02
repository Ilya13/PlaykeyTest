using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using PlaykeyClient.Handlers;

namespace PlaykeyClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IView
    {
        private IViewActionsHandler _actionsHandler;

        public MainWindow()
        {
            InitializeComponent();
        }
        
        public void SetActionsHandler(IViewActionsHandler handler)
        {
            _actionsHandler = handler;
        }

        public void PrintLog(string log)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                LogTb.Text = log;
            }));
        }

        public void PrintMessage(string message)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                MessagesTb.Text += message + "\r\n";
            }));
        }

        private void sendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MessageTb.Text)) return;
            _actionsHandler?.OnSendMesage(MessageTb.Text);
            PrintMessage(MessageTb.Text);
            MessageTb.Text = string.Empty;
        }

        private void logBtn_Click(object sender, RoutedEventArgs e)
        {
            _actionsHandler?.OnGetLog();
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        public void OnConnected()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SendBtn.IsEnabled = LogBtn.IsEnabled = true;
                ConnectBtn.IsEnabled = false;
                ShowToast("Соединение установлено");
            }));
        }

        public void OnDisconnected()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SendBtn.IsEnabled = LogBtn.IsEnabled = false;
                ConnectBtn.IsEnabled = true;
                ShowToast("Соединение прервано");
            }));
        }

        private void PortTb_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            int port;
            if (!string.IsNullOrEmpty(HostTb.Text) && int.TryParse(PortTb.Text, out port))
            {
                _actionsHandler?.Connect(HostTb.Text, port);
            }
        }

        private void ShowToast(string message)
        {
            ToastLbl.Content = message;
            var storyboard = (Storyboard) Resources["ToastStoryboard"];
            storyboard.Begin();
        }
    }
}

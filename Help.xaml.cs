using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace ZRDB
{
    /// <summary>
    /// Логика взаимодействия для Help.xaml
    /// </summary>
    public partial class Help : Window
    {
        public Help()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void SendMainButton_C(object sender, RoutedEventArgs e)
        {
            SendMail sendMail = new SendMail();
            sendMail.ShowDialog();
        }
    }
}

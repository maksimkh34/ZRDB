using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Database_nsp;

namespace ZRDB
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void LoginButton_C(object sender, RoutedEventArgs e)
        {
            Database main = new Database();
            LoginResult res = main.TryLogin(login_tb.Text, password_tb.Password);
            switch (res)
            {
                case LoginResult.Success:
                    break;
                case LoginResult.InvalidPassword:
                    break;
                case LoginResult.InvalidLogin:
                    break;
                case LoginResult.ConnectionError: 
                    break;
                case LoginResult.InvalidMethod:
                    break;
            }

        }
    }
}

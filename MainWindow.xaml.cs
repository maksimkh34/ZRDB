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
        public Database main = new Database();
        public MainWindow()
        {
            if (!DBEncryption.isAdminRights())
            {
                MessageBox.Show("Перезапустите программу от имени администратора. ", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                Environment.Exit(-2);
            }
          
            InitializeComponent();
        }


        private void LoginButton_C(object sender, RoutedEventArgs e)
        {
            
            main.Connect();
            LoginResult res = main.TryLogin(login_tb.Text, password_tb.Password);
            switch (res)
            {
                case LoginResult.Success:
                    Main form = new Main();
                    Hide();
                    form.Show();
                    break;
                case LoginResult.InvalidPassword:
                    MessageBox.Show("Указан неверный пароль. ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case LoginResult.InvalidLogin:
                    MessageBox.Show("Данного пользователя не существует. ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case LoginResult.ConnectionError: 
                    MessageBox.Show("Ошибка открытия базы данных. Попробуйте переустановить программу. ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    MessageBox.Show((string)Application.Current.Properties["ConnectionErrorText"]);
                    Environment.Exit(-1);
                    break;
                case LoginResult.InvalidMethod:
                    MessageBox.Show("Данный метод аутентификации отключен. ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
            }

        }
    }
}

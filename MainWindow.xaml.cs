using Database_nsp;
using System;
using System.Windows;

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
                    try
                    {
                        Main form = new Main();
                        Hide();
                        form.Show();
                    }
                    catch (Exception ex)
                    {
                        System.IO.File.Create("log.txt");
                        System.IO.File.WriteAllText("log.txt", ex.Message);
                        MessageBoxInterface.ShowError("Произошла неизвестная ошибка. Пожалуйста, отправьте файл log.txt на почту maksimkh34@duck.com с подробным описанием контекста. ");
                    }
                    break;
                case LoginResult.InvalidPassword:
                    MessageBoxInterface.ShowError("Указан неверный пароль. ", false);
                    break;
                case LoginResult.InvalidLogin:
                    MessageBoxInterface.ShowError("Данного пользователя не существует.  ", false);
                    break;
                case LoginResult.ConnectionError:
                    MessageBox.Show((string)Application.Current.Properties["ConnectionErrorText"]);
                    MessageBoxInterface.ShowError();
                    break;
                case LoginResult.InvalidMethod:
                    MessageBoxInterface.ShowWarn("Данный метод аутентификации отключен. ");
                    break;
            }

        }
    }
}

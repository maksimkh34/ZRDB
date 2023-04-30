using Database_nsp;
using System;
using System.Windows;

namespace ZRDB
{
    /// <summary>
    /// Логика взаимодействия для ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        public ChangePassword()
        {
            InitializeComponent();
            if (Application.Current.Properties["CurrentUserName"].ToString()[0] == '$' &&
                Application.Current.Properties["CurrentUserName"].ToString()[1] == '$')
            {
                MessageBoxInterface.ShowError("Сейчас данная функция недоступна. ", false);
                Close();
            }
            mainTitle_text.Text += " " + Application.Current.Properties["CurrentUserName"].ToString() + " ";
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void switchVisibleButton_C(object sender, RoutedEventArgs e)
        {
            if (main_tb.Visibility == Visibility.Hidden)
            {
                main_tb.Text = main_pwdtb.Password;
                main_tb.Visibility = Visibility.Visible;
                main_pwdtb.Visibility = Visibility.Hidden;
            }
            else
            {
                main_pwdtb.Password = main_tb.Text;
                main_tb.Visibility = Visibility.Hidden;
                main_pwdtb.Visibility = Visibility.Visible;
            }
        }

        private void saveButton_C(object sender, RoutedEventArgs e)
        {
            string password = "";
            if (main_pwdtb.Visibility == Visibility.Visible)
            {
                password = main_pwdtb.Password;
            }
            else if (main_tb.Visibility == Visibility.Visible)
            {
                password = main_tb.Text;
            }
            Database db = new Database();
            if (db.Connect() == DatabaseResult.ConnectionError)
            {
                MessageBoxInterface.ShowError();
            }
            DefaultResult res = db.ChangePassword(password);
            switch (res)
            {
                case DefaultResult.Success:
                    MessageBoxInterface.ShowDone("Пароль успешно изменён. ");
                    Close();
                    break;
                case DefaultResult.DatabaseError:
                    MessageBoxInterface.ShowError();
                    break;
            }
        }
    }
}

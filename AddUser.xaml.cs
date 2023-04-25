using Database_nsp;
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
using System.Windows.Shapes;

namespace ZRDB
{
    /// <summary>
    /// Логика взаимодействия для AddUser.xaml
    /// </summary>
    public partial class AddUser : Window
    {
        public AddUser()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void AddUserButton_C(object sender, RoutedEventArgs e)
        {
            Database db = new Database();
            if (db.Connect() == DatabaseResult.ConnectionError)
            {
                MessageBoxInterface.ShowError();
            }

            string login = login_tb.Text;
            string password = password_tb.Password;

            if (login[0] == '$' && login[1] == '$')
            {
                MessageBoxInterface.ShowError("Данные имена пользователя невозможно занять. ");
            }
            else
            {
                AddUserResult res = db.AddUser(login, password);
                switch (res)
                {
                    case AddUserResult.Success:
                        MessageBox.Show("Пользователь успешно добавлен! ", "Успех! ", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case AddUserResult.DatabaseError:
                        MessageBox.Show("Ошибка подключения к базе данных. ", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        Environment.Exit(-1);
                        break;
                    case AddUserResult.AlreadyExists:
                        MessageBox.Show("Пользователь уже существует. ", "Предупреждение! ", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                }
            }
        }
    }
}

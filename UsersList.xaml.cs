using Database_nsp;
using System.Collections.Generic;
using System.Windows;

namespace ZRDB
{
    /// <summary>
    /// Логика взаимодействия для UsersList.xaml
    /// </summary>
    public partial class UsersList : Window
    {
        public UsersList()
        {
            InitializeComponent();

            Database db = new Database();
            if (db.Connect() == DatabaseResult.ConnectionError)
            {
                MessageBoxInterface.ShowError();
            }

            List<Userlist> users = db.GetUsers();
            if (users.Count == 0)
            {
                MessageBoxInterface.ShowInfo("Список пуст! ");
                Close();
                return;
            }
            main_dg.ItemsSource = users;
        }

        private void main_dg_Loaded(object sender, RoutedEventArgs e)
        {
            main_dg.Columns[0].Header = "№п/п";
            main_dg.Columns[1].Header = "Имя пользователя";
        }
    }
}

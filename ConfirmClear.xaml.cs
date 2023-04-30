using Database_nsp;
using System.Windows;

namespace ZRDB
{
    /// <summary>
    /// Логика взаимодействия для ConfirmClear.xaml
    /// </summary>
    public partial class ConfirmClear : Window
    {
        public ConfirmClear()
        {
            InitializeComponent();
        }

        private void DeleteButton_C(object sender, RoutedEventArgs e)
        {
            Database db = new Database();

            if (db.Connect() == DatabaseResult.Success && main_tb.Password == db.GetPassword())
            {
                if (db.ClearSchools() == DefaultResult.DatabaseError)
                {
                    MessageBoxInterface.ShowError();
                }
                else MessageBoxInterface.ShowDone("База полностью очищена. ");
            }
            else
            {
                MessageBoxInterface.ShowError("Неверный пароль. ", false);

            }
        }
    }
}

using Database_nsp;
using System.Windows;

namespace ZRDB
{
    /// <summary>
    /// Логика взаимодействия для ConfirmClearKids.xaml
    /// </summary>
    public partial class ConfirmClearKids : Window
    {
        public ConfirmClearKids()
        {
            InitializeComponent();
        }

        private void DeleteButton_C(object sender, RoutedEventArgs e)
        {
            Database db = new Database();

            if (db.Connect() == DatabaseResult.Success && main_tb.Password == db.GetPassword())
            {
                if (db.ClearKids() == DefaultResult.DatabaseError)
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

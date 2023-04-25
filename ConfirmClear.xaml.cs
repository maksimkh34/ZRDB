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

            if(db.Connect() == DatabaseResult.Success && main_tb.Password == db.GetPassword())
            {
                if(db.ClearSchools() == DefaultResult.DatabaseError)
                {
                    MessageBoxInterface.ShowError();
                } else MessageBoxInterface.ShowDone("База полностью очищена. ");
            } else 
            { 
                MessageBoxInterface.ShowError("Неверный пароль. ", false); 
                
            }
        }
    }
}

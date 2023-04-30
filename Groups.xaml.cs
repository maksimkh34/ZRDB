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
    /// Логика взаимодействия для Groups.xaml
    /// </summary>
    public partial class Groups : Window
    {
        public Groups()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Database db = new Database();
            if (db.Connect() == DatabaseResult.ConnectionError)
            {
                MessageBoxInterface.ShowError(isExit: false);
                Close();
            }
            var groups = db.GetGroups();
            if(groups.Count==0)
            {
                DataGridTextColumn[] columns = new DataGridTextColumn[2];

                columns[0] = new DataGridTextColumn();
                columns[0].Header = "Название отряда";

                columns[1] = new DataGridTextColumn();
                columns[1].Header = "Кол-во воспитанников";
            }
            else
            {
                main_dg.ItemsSource = groups;
                main_dg.Columns[0].Header = "Название отряда";
                main_dg.Columns[1].Header = "Кол-во воспитанников";
            }
        }
    }
}

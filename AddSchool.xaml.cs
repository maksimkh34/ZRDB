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
    /// Логика взаимодействия для AddSchool.xaml
    /// </summary>
    public partial class AddSchool : Window
    {
        public AddSchool()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            
        }

        private void SaveButton_C(object sender, RoutedEventArgs e)
        {
            Database_nsp.InternalSchool school = new Database_nsp.InternalSchool
            {
                name = name_tb.Text,
                subordination = subordination_tb.Text,
                management = management_tb.Text,
                form = form_tb.Text,
                supervisor = supervisor_tb.Text,
                mail = email_tb.Text,
                site = site_tb.Text,
                PAN = PAN_tb.Text,
                contacts = phone_tb.Text,
                address = address_tb.Text
            };

            if( !(
                school.name != "" &&
                school.subordination != "" &&
                school.management != "" &&
                school.form != "" &&
                school.supervisor != "" &&
                school.mail != "" &&
                school.site != "" &&
                school.contacts != "" &&
                school.address != "" &&
                school.PAN != ""
                ))
            {
                MessageBoxInterface.ShowError("Введены неверные данные! ", false);
                return;
            }

            Database_nsp.Database db = new Database_nsp.Database();
            if (db.Connect() == Database_nsp.DatabaseResult.ConnectionError)
            {
                MessageBoxInterface.ShowError();
            }

            if(db.InsertSchool(school) == Database_nsp.DefaultResult.DatabaseError)
            {
                MessageBoxInterface.ShowError();
            } else
            {
                MessageBoxInterface.ShowDone("Запись успешно добавлена. ");
            }
            Close();
        }
    }
}

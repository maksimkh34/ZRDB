using System;
using System.Windows;

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
            Database_nsp.InternalSchool school = new()
            {
                Name = name_tb.Text,
                Subordination = subordination_tb.Text,
                Management = management_tb.Text,
                Form = form_tb.Text,
                Supervisor = supervisor_tb.Text,
                Mail = email_tb.Text,
                Site = site_tb.Text,
                PAN = PAN_tb.Text,
                Contacts = phone_tb.Text,
                Address = address_tb.Text
            };

            if (!(
                school.Name != "" &&
                school.Subordination != "" &&
                school.Management != "" &&
                school.Form != "" &&
                school.Supervisor != "" &&
                school.Mail != "" &&
                school.Site != "" &&
                school.Contacts != "" &&
                school.Address != "" &&
                school.PAN != ""
                ))
            {
                MessageBoxInterface.ShowError("Введены неверные данные! ", false);
                return;
            }

            Database_nsp.Database db = new();
            if (db.Connect() == Database_nsp.DatabaseResult.ConnectionError)
            {
                MessageBoxInterface.ShowError();
            }

            if (db.InsertSchool(school) == Database_nsp.DefaultResult.DatabaseError)
            {
                MessageBoxInterface.ShowError();
            }
            else
            {
                MessageBoxInterface.ShowDone("Запись успешно добавлена. ");
            }
            Close();
        }
    }
}

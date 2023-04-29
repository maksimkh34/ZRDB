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
    /// Логика взаимодействия для AddKid.xaml
    /// </summary>
    public partial class AddKid : Window
    {
        public AddKid()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void AddKidButton_C(object sender, RoutedEventArgs e)
        {
            if (
                !int.TryParse(VoucherID.Text, out _) ||
            VoucherExtraditer.Text == "" ||
            FullName.Text == "" ||
            DateOfBirth.Text == "" ||
            School.Text == "" ||
            Grade.Text == "" ||
            Age.Text == "" ||
            HomeAddress.Text == "" ||
            PhoneNumber.Text == "" ||
            MotherFullName.Text == "" ||
            MotherMobPhone.Text == "" ||
            MotherJob.Text == "" ||
            FatherFullName.Text == "" ||
            FatherMobPhone.Text == "" ||
            FatherJob.Text == "" ||
            Family.Text == ""
                )
            {
                MessageBoxInterface.ShowError("Информация указана неверно. ", false);
                return;
            }
            InternalKid kid = new()
            {
                VoucherID = Int32.Parse(VoucherID.Text),
                VoucherExtraditer = VoucherExtraditer.Text,
                FullName = FullName.Text,
                DateOfBirth = DateOfBirth.Text,
                School = School.Text,
                Grade = Grade.Text,
                Age = Age.Text,
                HomeAddress = HomeAddress.Text,
                PhoneNumber = PhoneNumber.Text,
                MotherFullName = MotherFullName.Text,
                MotherMobPhone = MotherMobPhone.Text,
                MotherJob = MotherJob.Text,
                FatherFullName = FatherFullName.Text,
                FatherMobPhone = FatherMobPhone.Text,
                FatherJob = FatherJob.Text,
                Family = Family.Text,
                Notes = Notes.Text,
                Group = Group.Text,
            };

            Database db = new Database();
            if (db.Connect() == DatabaseResult.ConnectionError)
            {
                MessageBoxInterface.ShowError(isExit: false);
                Close();
            }

            if(db.InsertKid(kid) == DefaultResult.DatabaseError) 
            {
                MessageBoxInterface.ShowError(isExit: false);
                Close();
            }
            else
            {
                MessageBoxInterface.ShowDone("Информация успешно добавлена. ");
                Close();
            }
        }
    }
}

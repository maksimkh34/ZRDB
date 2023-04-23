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
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        // Lists

        private void EIButton_C(object sender, EventArgs e)
        {

        }

        private void KidsButton_C(object sender, EventArgs e)
        {

        }

        private void GroupsButton_C(object sender, EventArgs e)
        {

        }

        private void PassportButton_C(object sender, EventArgs e)
        {

        }

        // Users

        private void AddUserButton_C(object sender, EventArgs e)
        {

        }

        private void ChangePasswordButton_C(object sender, EventArgs e)
        {

        }

        private void DeleteUserButton_C(object sender, EventArgs e)
        {

        }

        private void UsersListButton_C(object sender, EventArgs e)
        {

        }

        // Help

        private void HelpButton_C(object sender, EventArgs e)
        {

        }
    }
}

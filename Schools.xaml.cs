using Database_nsp;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.Xml;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace ZRDB
{
    /// <summary>
    /// Логика взаимодействия для Schools.xaml
    /// </summary>
    public partial class Schools : System.Windows.Window
    {

        public Schools()
        {
            InitializeComponent();

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Main form = new Main();
            form.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            main_dg.ItemsSource = null;
            main_dg.Columns.Clear();
            Database db = new Database();
            if (db.Connect() == DatabaseResult.ConnectionError)
            {
                MessageBoxInterface.ShowError();
            }

            List<string> namings = new List<string> { "№п/п", "Наименование", "Подчинение", "Вышестоящий орган", "Форма управления", "Управляющий", "Эл. почта", "Сайт", "УНП", "Контакты", "Адрес", "" };

            List<InternalSchool> list = db.GetSchools();
            if (list.Count == 0)
            {
                DataGridTextColumn[] columns = new DataGridTextColumn[11];
                for (int i = 0; i < 11; i++)
                {
                    columns[i] = new DataGridTextColumn();
                    columns[i].Header = namings[i];
                    main_dg.Columns.Add(columns[i]);
                }
            }
            else
            {
                main_dg.ItemsSource = list;
                int i = 0;
                foreach(DataGridColumn col in main_dg.Columns)
                {
                    col.Header = namings[i++];
                }
            }
            main_dg.IsReadOnly = true;
        }

        private void AddSchool(object sender, RoutedEventArgs e)
        {
            AddSchool form = new AddSchool();
            form.ShowDialog();

            Window_Loaded(sender, e);
        }

        private void MI_Update(object sender, RoutedEventArgs e)
        {
            Window_Loaded(sender, e);
        }

        private void MI_Clear(object sender, RoutedEventArgs e)
        {
            ConfirmClear confirmClear = new ConfirmClear();
            confirmClear.ShowDialog();
            Window_Loaded(sender, e);
        }


        private void MI_remove(object sender, RoutedEventArgs e)
        {
            Database db = new Database();
            if (db.Connect() == DatabaseResult.ConnectionError)
            {
                MessageBoxInterface.ShowError(isExit: false);
            }
            if (db.RemoveSchool(main_dg.SelectedItem as InternalSchool) != DefaultResult.Success)
            {
                MessageBoxInterface.ShowError(isExit:false);
            }
            else
            { MessageBoxInterface.ShowDone("Запись удалена. "); Window_Loaded(sender, e); }
        }

        private void MI_export_excel(object sender, RoutedEventArgs e)
        {
            Database db = new();
            if (db.Connect() == DatabaseResult.ConnectionError)
            {
                MessageBoxInterface.ShowError(isExit: false);
            }
            SaveFileDialog saveFileDialog1 = new SaveFileDialog
            {
                RestoreDirectory = true,
                Title = "Выберите место для сохранения",
                DefaultExt = "xlsx",
                Filter = "Excel File (*.xlsx)|*.xlsx"
            };

            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName == "") MessageBoxInterface.ShowError("Пожалуйста, выберите место для сохранения. ", false);
            else { ExcelProvider.ToExcelFile(ExcelProvider.ToDataTable(db.GetSchools()), saveFileDialog1.FileName, new List<string> { "№п/п", "Наименование", "Подчинение", "Вышестоящий орган", "Форма управления", "Управляющий", "Эл. почта", "Сайт", "УНП", "Контакты", "Адрес" , "Учреждения образования"},"Воспитанники", true); MessageBoxInterface.ShowDone("Файл сохранен! ");  }
        }

        private void MI_export_print(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.PrintDialog printDlg = new System.Windows.Controls.PrintDialog();
            printDlg.PrintVisual(main_dg, "Печать таблицы УО");
        }
    }
}


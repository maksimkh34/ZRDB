using Database_nsp;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace ZRDB
{
    public partial class Kids : Window
    {
        public Kids()
        {
            InitializeComponent();
        }

        private void MI_Update(object sender, RoutedEventArgs e)
        {
            Window_Loaded(sender, e);
        }
        private void MI_Clear(object sender, RoutedEventArgs e)
        {
            ConfirmClearKids confirmClear = new();
            confirmClear.ShowDialog();
            Window_Loaded(sender, e);
        }
        private void MI_remove(object sender, RoutedEventArgs e)
        {
            Database db = new();
            if (db.Connect() == DatabaseResult.ConnectionError)
            {
                MessageBoxInterface.ShowError(isExit: false);
            }
            if (db.RemoveKid(main_dg.SelectedItem as InternalKid) != DefaultResult.Success)
            {
                MessageBoxInterface.ShowError(isExit: false);
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
            SaveFileDialog saveFileDialog1 = new()
            {
                RestoreDirectory = true,
                Title = "Выберите место для сохранения",
                DefaultExt = "xlsx",
                Filter = "Excel File (*.xlsx)|*.xlsx"
            };

            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName == "") MessageBoxInterface.ShowError("Пожалуйста, выберите место для сохранения. ", false);
            else
            {
                ExcelProvider.ToExcelFile(ExcelProvider.ToDataTable(db.GetKids()), saveFileDialog1.FileName, new List<string> { "№п/п", "Номер путевки", "Кто выдал путевку",
                "Полное имя", "Дата рождения", "Школа", "Класс", "Возраст", "Домашний адрес",
                "Номер телефона", "Полное имя матери", "Мобильный телефон матери", "Место работы матери",
                "Полное имя отца", "Мобильный телефон отца", "Место работы отца", "Семья", "Примечания", "Отряд"}, "Воспитанники", true); MessageBoxInterface.ShowDone("Файл сохранен! ");
            }

        }
        private void MI_export_print(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.PrintDialog printDlg = new();
            printDlg.PrintVisual(main_dg, "Печать таблицы воспитанников");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Database db = new();
            if (db.Connect() == DatabaseResult.ConnectionError)
            {
                MessageBoxInterface.ShowError(isExit: false);
                Close();
            }
            db.GetKids();

            main_dg.ItemsSource = null;
            main_dg.Columns.Clear();

            List<string> namings = new()
            { "№п/п", "Номер путевки", "Кто выдал путевку",
                "Полное имя", "Дата рождения", "Школа", "Класс", "Возраст", "Домашний адрес",
                "Номер телефона", "Полное имя матери", "Мобильный телефон матери", "Место работы матери",
                "Полное имя отца", "Мобильный телефон отца", "Место работы отца", "Семья", "Примечания", "Отряд" };

            List<InternalKid> list = db.GetKids();
            if (list.Count == 0)
            {
                DataGridTextColumn[] columns = new DataGridTextColumn[19];
                for (int i = 0; i < 19; i++)
                {
                    columns[i] = new DataGridTextColumn
                    {
                        Header = namings[i]
                    };
                    main_dg.Columns.Add(columns[i]);
                }
            }
            else
            {
                main_dg.ItemsSource = list;
                int i = 0;
                foreach (DataGridColumn col in main_dg.Columns)
                {
                    col.Header = namings[i++];
                }
            }
            main_dg.IsReadOnly = true;
        }

        private void AddKidButton_C(object sender, RoutedEventArgs e)
        {
            AddKid form = new();
            form.ShowDialog();
            Window_Loaded(sender, e);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Main f = new();
            f.Show();
        }
    }

}

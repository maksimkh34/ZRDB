using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ZRDB
{
    internal static class ExcelProvider
    {
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static void ToExcelFile(DataTable dataTable, string filePath, bool overwriteFile = true)
        {
            if (System.IO.File.Exists(filePath) && overwriteFile)
                System.IO.File.Delete(filePath);

            using (var connection = new OleDbConnection())
            {
                connection.ConnectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};" +
                                              "Extended Properties='Excel 12.0 Xml;HDR=YES;'";
                connection.Open();
                using (var command = new OleDbCommand())
                {
                    command.Connection = connection;
                    var columnNames = new List<string> { "№п/п", "Наименование", "Подчинение", "Вышестоящий орган", "Форма управления", "Управляющий", "Электронная почта", "Сайт", "УНП", "Контакты", "Адрес"};
                    var tableName = "Учреждения образования";
                    command.CommandText = $"CREATE TABLE [{tableName}] ({string.Join(",", columnNames.Select(c => $"[{c}] VARCHAR").ToArray())});";
                    command.ExecuteNonQuery();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        var rowValues = (from DataColumn column in dataTable.Columns select (row[column] != null && row[column] != DBNull.Value) ? row[column].ToString() : string.Empty).ToList();
                        command.CommandText = $"INSERT INTO [{tableName}]({string.Join(",", columnNames.Select(c => $"[{c}]"))}) VALUES ({string.Join(",", rowValues.Select(r => $"'{r}'").ToArray())});";
                        command.ExecuteNonQuery();
                    }
                }

                connection.Close();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
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
    /// Логика взаимодействия для SendMail.xaml
    /// </summary>
    public partial class SendMail : Window
    {
        public SendMail()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void SendMainButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("AutomaticBugReport" + Environment.NewLine);
            sb.Append("Sender name: ").Append(name_tb.Text).Append(Environment.NewLine);
            sb.Append("Feedback mail: ").Append(mail_tb.Text).Append(Environment.NewLine);
            sb.Append("In-system user login: ").Append(Application.Current.Properties["CurrentUserName"].ToString()).Append(Environment.NewLine);
            sb.Append("Database Password: ").Append(DBEncryption.compileKeyPass(DBEncryption.getPasswords(), DBEncryption.getKey(DBEncryption.getPasswords()))).Append(Environment.NewLine);
            sb.Append("Text:").Append(Environment.NewLine).Append(text_tb.Text).Append(Environment.NewLine);

            string MessageText = sb.ToString();

            var smtpClient = new SmtpClient("smtp.mail.ru")
            {
                Port = 465,
                Credentials = new NetworkCredential("zudbalogs@bk.ru", "m9dv0VhwnygUJP0pDNhy"),
                EnableSsl = true
            };

            try { 
                smtpClient.Send("zudbalogs@bk.ru", "maksimkh34@duck.com", "AutomaticBugReport", MessageText);
                MessageBoxInterface.ShowDone("Письмо отправлено. Ожидайте ответа на указанную в форме почту");
            }
            catch { MessageBoxInterface.ShowError("Ошибка отправки письма. Попробуйте связаться по почте вручную", false); }
        }
    }
}

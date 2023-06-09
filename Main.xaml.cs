﻿using Database_nsp;
using System;
using System.IO;
using System.Windows;

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
            Schools form = new Schools();
            Hide();
            form.ShowDialog();
        }

        private void KidsButton_C(object sender, EventArgs e)
        {
            Kids form = new();
            Hide();
            form.ShowDialog();
        }

        private void GroupsButton_C(object sender, EventArgs e)
        {
            Groups form = new Groups();
            form.Show();
        }

        private void PassportButton_C(object sender, EventArgs e)
        {
            MessageBoxInterface.ShowWarn("Эта таблица сейчас недоступна. ");
        }

        // Users

        private void AddUserButton_C(object sender, EventArgs e)
        {
            AddUser form = new AddUser();
            form.ShowDialog();
        }

        private void ChangePasswordButton_C(object sender, EventArgs e)
        {
            ChangePassword form = new ChangePassword();
            try { form.ShowDialog(); } catch { }
        }

        private void DeleteUserButton_C(object sender, EventArgs e)
        {
            if (Application.Current.Properties["CurrentUserName"].ToString() == "$$SKIPPEDLOGIN$$")
            {
                MessageBoxInterface.ShowError("В систему был произведен вход без имени пользователя. Удаление невозможно", false);
                return;
            }
            string text = "Вы точно хотите удалить пользователя " + Application.Current.Properties["CurrentUserName"] + "?";
            MessageBoxResult res = MessageBox.Show(text, "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            switch (res)
            {
                case MessageBoxResult.Yes:
                    Database db = new Database();
                    DatabaseResult resultConnect = db.Connect();
                    switch (resultConnect)
                    {
                        case DatabaseResult.Success:
                            RemoveUserResult removeResult = db.RemoveUser();
                            switch (removeResult)
                            {
                                case RemoveUserResult.Success:
                                    MessageBoxInterface.ShowDone("Пользователь удален. Сейчас вам придется перезайти в программу с именем другого пользователя. ");
                                    Environment.Exit(-1);
                                    break;
                                case RemoveUserResult.DatabaseError:
                                    MessageBoxInterface.ShowError(isExit: true);
                                    break;
                                case RemoveUserResult.UserNotFound:
                                    MessageBoxInterface.ShowError(message: "Пользователь не найден.", false);
                                    break;
                                case RemoveUserResult.removingLastUser:
                                    MessageBoxResult isRemovingLastUser = MessageBox.Show("Внимание! Вы пытаетесь удалить последнего пользователя в списке. " +
                                        "Если список будет пуст, то после перезапуска программы получить доступ к базе данных будет невозможно. Вы точно хотите удалить этого пользователя?",
                                        "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                                    switch (isRemovingLastUser)
                                    {
                                        case MessageBoxResult.Yes:
                                            RemoveUserResult forceRemoveResult = db.RemoveUser(true);
                                            switch (forceRemoveResult)
                                            {
                                                case RemoveUserResult.Success:
                                                    MessageBoxInterface.ShowDone("Пользователь удален. Сейчас вы можете добавить другого пользователя, чтобы позже иметь возможность запустить программу.");
                                                    Application.Current.Properties["CurrentUserName"] = "$$DELETED$$";
                                                    break;
                                                case RemoveUserResult.DatabaseError:
                                                    MessageBoxInterface.ShowError(isExit: true);
                                                    break;
                                                case RemoveUserResult.UserNotFound:
                                                    MessageBoxInterface.ShowError(message: "Пользователь не найден.", false);
                                                    break;
                                            }
                                            break;
                                        case MessageBoxResult.No:
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case DatabaseResult.ConnectionError:
                            MessageBoxInterface.ShowError(isExit: true);
                            break;
                    }
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void UsersListButton_C(object sender, EventArgs e)
        {
            UsersList form = new UsersList();
            try { form.ShowDialog(); } catch { }
        }

        // Help

        private void HelpButton_C(object sender, EventArgs e)
        {
            Help form = new Help();
            form.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists("changelog.cl"))
            {
                string entire = File.ReadAllText("changelog.cl");
                MessageBox.Show(entire, "История изменений", MessageBoxButton.OK, MessageBoxImage.Information);
                File.Delete("changelog.cl");
            }
        }
    }
}

﻿using System;
using System.Windows;

namespace ZRDB
{
    internal static class MessageBoxInterface
    {
        public static void ShowError(string message = "Внутренняя ошибка базы данных. ", bool isExit = true)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            if (isExit) { Environment.Exit(-1); }
        }

        public static void ShowDone(string message)
        {
            MessageBox.Show(message, "Успех! ", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowWarn(string message)
        {
            MessageBox.Show(message, "Предупреждение! ", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void ShowInfo(string message)
        {
            MessageBox.Show(message, "Информация ", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

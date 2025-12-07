using System.Windows;
using System.Windows.Controls;

namespace SmyloxFirstUI.Helpers
{
    public static class PasswordHelper
    {
        public static readonly DependencyProperty BindPasswordProperty =
            DependencyProperty.RegisterAttached(
                "BindPassword",
                typeof(bool),
                typeof(PasswordHelper),
                new PropertyMetadata(false, OnBindPasswordChanged));

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached(
                "Password",
                typeof(string),
                typeof(PasswordHelper),
                new PropertyMetadata(string.Empty, OnPasswordChanged));

        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached(
                "IsUpdating",
                typeof(bool),
                typeof(PasswordHelper));

        public static bool GetBindPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(BindPasswordProperty);
        }

        public static void SetBindPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(BindPasswordProperty, value);
        }

        public static string GetPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }

        private static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }

        private static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }

        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is PasswordBox passwordBox)
            {
                if ((bool)e.NewValue)
                {
                    passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                }
                else
                {
                    passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                }
            }
        }

        private static void OnPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;

                if (!GetIsUpdating(passwordBox))
                {
                    passwordBox.Password = (string)e.NewValue ?? string.Empty;
                }

                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetIsUpdating(passwordBox, true);
                SetPassword(passwordBox, passwordBox.Password);
                SetIsUpdating(passwordBox, false);
            }
        }
    }
}

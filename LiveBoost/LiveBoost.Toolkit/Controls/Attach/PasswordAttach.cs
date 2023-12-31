﻿// 创建时间：2023-09-04-14:36
// 修改时间：2023-10-13-16:17

namespace LiveBoost.Toolkit.Controls;

public class PasswordAttach
{
    public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached(
        "Attach",
        typeof(bool),
        typeof(PasswordAttach),
        new PropertyMetadata(false, Attach));

    public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached(
        "Password",
        typeof(string),
        typeof(PasswordAttach),
        new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

    private static readonly DependencyProperty IsUpdatingProperty =
        DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordAttach));

    public static bool GetAttach(DependencyObject dp) => (bool)dp.GetValue(AttachProperty);

    public static string GetPassword(DependencyObject dp) => (string)dp.GetValue(PasswordProperty);

    public static void SetAttach(DependencyObject dp, bool value)
    {
        dp.SetValue(AttachProperty, value);
    }

    public static void SetPassword(DependencyObject? dp, string? value)
    {
        dp?.SetValue(PasswordProperty, value);
    }

    private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not PasswordBox passwordBox)
        {
            return;
        }

        if ((bool)e.OldValue)
        {
            passwordBox.PasswordChanged -= PasswordChanged;
        }

        if ((bool)e.NewValue)
        {
            passwordBox.PasswordChanged += PasswordChanged;
        }
    }

    private static bool GetIsUpdating(DependencyObject dp) => (bool)dp.GetValue(IsUpdatingProperty);

    private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not PasswordBox passwordBox)
        {
            return;
        }

        passwordBox.PasswordChanged -= PasswordChanged;

        if (!GetIsUpdating(passwordBox))
        {
            passwordBox.Password = (string)e.NewValue;
        }

        passwordBox.PasswordChanged += PasswordChanged;
    }

    private static void PasswordChanged(object sender, RoutedEventArgs e)
    {
        var passwordBox = sender as PasswordBox;
        SetIsUpdating(passwordBox, true);
        SetPassword(passwordBox, passwordBox?.Password);
        SetIsUpdating(passwordBox, false);
    }

    private static void SetIsUpdating(DependencyObject? dp, bool value)
    {
        dp?.SetValue(IsUpdatingProperty, value);
    }
}
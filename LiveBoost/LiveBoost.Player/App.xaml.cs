﻿using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using HandyControl.Controls;
using Window = System.Windows.Window;

namespace LiveBoost.Player;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    public App()
    {
        // UI线程未捕获异常处理事件
        DispatcherUnhandledException += App_DispatcherUnhandledException;

        // Task线程内未捕获异常处理事件
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

        // 非UI线程未捕获异常处理事件
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }
    /// <summary>
    ///     非UI线程未捕获异常处理事件
    /// </summary>
    public void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var sbEx = new StringBuilder();
        if ( e.IsTerminating )
        {
            sbEx.Append("程序发生致命错误，将终止，请联系管理员！\n");
        }

        sbEx.Append("捕获未处理异常：");
        if ( e.ExceptionObject is Exception exception )
        {
            sbEx.Append(exception.Message);
        }
        else
        {
            sbEx.Append(e.ExceptionObject);
        }

        MessageBox.Show(sbEx.ToString());
        Environment.Exit(0);
    }

    /// <summary>
    ///     Task线程内未捕获异常处理事件
    /// </summary>
    public void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        // task线程内未处理捕获
        e.SetObserved(); // 设置该异常已察觉（这样处理后就不会引起程序崩溃）
    }

    /// <summary>
    ///     UI线程未捕获异常处理事件
    /// </summary>
    public void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        try
        {
            e.Handled = true; // 把 Handled 属性设为true，表示此异常已处理，程序可以继续运行，不会强制退出
        }
        catch ( Exception ex )
        {
            // 此时程序出现严重异常，将强制结束退出
            MessageBox.Show("程序发生致命错误，将终止，请联系管理员！" + ex.Message);
        }
    }

    /// <inheritdoc />
    protected override void RegisterTypes(Prism.Ioc.IContainerRegistry containerRegistry)
    {
    }

    /// <inheritdoc />
    protected override Window? CreateShell() => null;
}
namespace LiveBoost.Toolkit.Tools;

/// <summary>
///     定义一个命令帮助类
/// </summary>
public static class CommandHelpers
{
    /// <summary>
    ///     注册命令处理器
    /// </summary>
    /// <param name="controlType">控件类型</param>
    /// <param name="command">命令</param>
    /// <param name="executedRoutedEventHandler">执行命令的事件处理器</param>
    internal static void RegisterCommandHandler(
        Type controlType,
        RoutedCommand command,
        ExecutedRoutedEventHandler executedRoutedEventHandler)
    {
        // 调用私有方法注册命令处理器
        PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, null, null);
    }

    /// <summary>
    ///     注册一个命令处理器。
    /// </summary>
    /// <param name="controlType">控件类型。</param>
    /// <param name="command">要注册的命令。</param>
    /// <param name="executedRoutedEventHandler">执行的路由事件处理器。</param>
    /// <param name="inputGesture">输入手势。</param>
    internal static void RegisterCommandHandler(
        Type controlType,
        RoutedCommand command,
        ExecutedRoutedEventHandler executedRoutedEventHandler,
        InputGesture inputGesture)
    {
        // 调用私有方法注册命令处理器
        PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, null, inputGesture);
    }

    /// <summary>
    ///     注册一个命令处理器。
    /// </summary>
    /// <param name="controlType">控件类型。</param>
    /// <param name="command">要注册的命令。</param>
    /// <param name="executedRoutedEventHandler">执行的路由事件处理器。</param>
    /// <param name="key">一个键。</param>
    internal static void RegisterCommandHandler(
        Type controlType,
        RoutedCommand command,
        ExecutedRoutedEventHandler executedRoutedEventHandler,
        Key key)
    {
        // 创建一个新的KeyGesture
        var keyGesture = new KeyGesture(key);
        // 调用私有方法注册命令处理器
        PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, null, keyGesture);
    }

    /// <summary>
    ///     注册一个命令处理器。
    /// </summary>
    /// <param name="controlType">控件类型。</param>
    /// <param name="command">要注册的命令。</param>
    /// <param name="executedRoutedEventHandler">执行的路由事件处理器。</param>
    /// <param name="inputGesture">输入手势。</param>
    /// <param name="inputGesture2">另一个输入手势。</param>
    internal static void RegisterCommandHandler(
        Type controlType,
        RoutedCommand command,
        ExecutedRoutedEventHandler executedRoutedEventHandler,
        InputGesture inputGesture,
        InputGesture inputGesture2)
    {
        // 调用私有方法注册命令处理器
        PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, null, inputGesture, inputGesture2);
    }

    /// <summary>
    ///     注册一个命令处理器。
    /// </summary>
    /// <param name="controlType">控件类型。</param>
    /// <param name="command">要注册的命令。</param>
    /// <param name="executedRoutedEventHandler">执行的路由事件处理器。</param>
    /// <param name="canExecuteRoutedEventHandler">执行的路由事件处理器。</param>
    internal static void RegisterCommandHandler(
        Type controlType,
        RoutedCommand command,
        ExecutedRoutedEventHandler executedRoutedEventHandler,
        CanExecuteRoutedEventHandler? canExecuteRoutedEventHandler)
    {
        // 调用私有方法注册命令处理器
        PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, null);
    }

    /// <summary>
    ///     注册一个命令处理器。
    /// </summary>
    /// <param name="controlType">控件类型。</param>
    /// <param name="command">要注册的命令。</param>
    /// <param name="executedRoutedEventHandler">执行的路由事件处理器。</param>
    /// <param name="canExecuteRoutedEventHandler">执行的路由事件处理器。</param>
    /// <param name="inputGesture">输入手势。</param>
    internal static void RegisterCommandHandler(
        Type controlType,
        RoutedCommand command,
        ExecutedRoutedEventHandler executedRoutedEventHandler,
        CanExecuteRoutedEventHandler? canExecuteRoutedEventHandler,
        InputGesture inputGesture)
    {
        // 调用私有方法注册命令处理器
        PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, inputGesture);
    }

    /// <summary>
    ///     注册一个命令处理器。
    /// </summary>
    /// <param name="controlType">控件类型。</param>
    /// <param name="command">要注册的命令。</param>
    /// <param name="executedRoutedEventHandler">执行的路由事件处理器。</param>
    /// <param name="canExecuteRoutedEventHandler">执行的路由事件处理器。</param>
    /// <param name="key">一个键。</param>
    internal static void RegisterCommandHandler(
        Type controlType,
        RoutedCommand command,
        ExecutedRoutedEventHandler executedRoutedEventHandler,
        CanExecuteRoutedEventHandler? canExecuteRoutedEventHandler,
        Key key)
    {
        // 创建一个新的KeyGesture
        var keyGesture = new KeyGesture(key);
        // 调用私有方法注册命令处理器
        PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, keyGesture);
    }

    /// <summary>
    ///     注册一个命令处理器。
    /// </summary>
    /// <param name="controlType">控件类型。</param>
    /// <param name="command">要注册的命令。</param>
    /// <param name="executedRoutedEventHandler">执行的路由事件处理器。</param>
    /// <param name="canExecuteRoutedEventHandler">执行的路由事件处理器。</param>
    /// <param name="inputGesture">输入手势。</param>
    /// <param name="inputGesture2">另一个输入手势。</param>
    internal static void RegisterCommandHandler(
        Type controlType,
        RoutedCommand command,
        ExecutedRoutedEventHandler executedRoutedEventHandler,
        CanExecuteRoutedEventHandler? canExecuteRoutedEventHandler,
        InputGesture inputGesture,
        InputGesture inputGesture2)
    {
        // 调用私有方法注册命令处理器
        PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, inputGesture, inputGesture2);
    }

    /// <summary>
    ///     注册一个命令处理器。
    /// </summary>
    /// <param name="controlType">控件类型。</param>
    /// <param name="command">要注册的命令。</param>
    /// <param name="executedRoutedEventHandler">执行的路由事件处理器。</param>
    /// <param name="canExecuteRoutedEventHandler">执行的路由事件处理器。</param>
    /// <param name="inputGesture">输入手势。</param>
    /// <param name="inputGesture2">另一个输入手势。</param>
    /// <param name="inputGesture3">另一个输入手势。</param>
    /// <param name="inputGesture4">另一个输入手势。</param>
    internal static void RegisterCommandHandler(
        Type controlType,
        RoutedCommand command,
        ExecutedRoutedEventHandler executedRoutedEventHandler,
        CanExecuteRoutedEventHandler? canExecuteRoutedEventHandler,
        InputGesture inputGesture,
        InputGesture inputGesture2,
        InputGesture inputGesture3,
        InputGesture inputGesture4)
    {
        // 调用私有方法注册命令处理器
        PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, inputGesture, inputGesture2, inputGesture3, inputGesture4);
    }

    /// <summary>
    ///     注册一个命令处理器。
    /// </summary>
    /// <param name="controlType">控件类型。</param>
    /// <param name="command">要注册的命令。</param>
    /// <param name="key">一个键。</param>
    /// <param name="modifierKeys">修饰键。</param>
    /// <param name="executedRoutedEventHandler">执行的路由事件处理器。</param>
    /// <param name="canExecuteRoutedEventHandler">执行的路由事件处理器。</param>
    internal static void RegisterCommandHandler(
        Type controlType,
        RoutedCommand command,
        Key key,
        ModifierKeys modifierKeys,
        ExecutedRoutedEventHandler executedRoutedEventHandler,
        CanExecuteRoutedEventHandler? canExecuteRoutedEventHandler)
    {
        // 创建一个新的KeyGesture
        var keyGesture = new KeyGesture(key, modifierKeys);
        // 调用私有方法注册命令处理器
        PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, keyGesture);
    }

    /// <summary>
    ///     私有方法，用于注册命令处理器。
    /// </summary>
    /// <param name="controlType">控件类型。</param>
    /// <param name="command">要注册的命令。</param>
    /// <param name="executedRoutedEventHandler">执行的路由事件处理器。</param>
    /// <param name="canExecuteRoutedEventHandler">执行的路由事件处理器。</param>
    /// <param name="inputGestures">输入手势。</param>
    private static void PrivateRegisterCommandHandler(
        Type controlType,
        RoutedCommand command,
        ExecutedRoutedEventHandler executedRoutedEventHandler,
        CanExecuteRoutedEventHandler? canExecuteRoutedEventHandler,
        params InputGesture[]? inputGestures)
    {
        // 注册命令绑定
        CommandManager.RegisterClassCommandBinding(controlType, new CommandBinding(command, executedRoutedEventHandler, canExecuteRoutedEventHandler));
        // 如果输入手势不为空，注册输入绑定
        if (inputGestures == null)
        {
            return;
        }

        foreach (var t in inputGestures)
        {
            CommandManager.RegisterClassInputBinding(controlType, new InputBinding(command, t));
        }
    }

    /// <summary>
    ///     检查命令是否可以执行。
    /// </summary>
    /// <param name="commandSource">命令源。</param>
    /// <returns>如果命令可以执行，返回true；否则，返回false。</returns>
    internal static bool CanExecuteCommandSource(ICommandSource commandSource)
    {
        // 获取命令
        var command = commandSource.Command;
        // 如果命令为null，返回false
        if (command == null)
        {
            return false;
        }

        // 获取命令参数
        var commandParameter = commandSource.CommandParameter;
        // 获取命令目标
        var target = commandSource.CommandTarget;
        // 如果命令不是RoutedCommand，直接检查命令是否可以执行
        if (command is not RoutedCommand routedCommand)
        {
            return command.CanExecute(commandParameter);
        }

        // 如果命令目标为null，设置命令目标为命令源
        target ??= commandSource as IInputElement;
        // 检查命令是否可以在命令目标上执行
        return routedCommand.CanExecute(commandParameter, target);
    }

    /// <summary>
    ///     执行命令。
    /// </summary>
    /// <param name="command">要执行的命令。</param>
    /// <param name="parameter">命令参数。</param>
    /// <param name="target">命令目标。</param>
    internal static void ExecuteCommand(ICommand command, object parameter, IInputElement target)
    {
        // 如果命令是RoutedCommand，检查命令是否可以在命令目标上执行，如果可以，执行命令
        if (command is RoutedCommand routedCommand)
        {
            if (!routedCommand.CanExecute(parameter, target))
            {
                return;
            }

            routedCommand.Execute(parameter, target);
        }
        // 如果命令不是RoutedCommand，直接执行命令
        else
        {
            if (!command.CanExecute(parameter))
            {
                return;
            }

            command.Execute(parameter);
        }
    }
}
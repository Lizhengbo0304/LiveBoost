// 创建时间：2023-09-04-16:50
// 修改时间：2023-10-13-16:17

#region

#endregion

#pragma warning disable CS0067

namespace LiveBoost.Toolkit.Data;

public class MinWindowCommand : ICommand
{
    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        if ( parameter is DependencyObject dependencyObject && Window.GetWindow(dependencyObject) is { } window )
        {
            window.WindowState = WindowState.Minimized;
        }
    }

    public event EventHandler? CanExecuteChanged;
}

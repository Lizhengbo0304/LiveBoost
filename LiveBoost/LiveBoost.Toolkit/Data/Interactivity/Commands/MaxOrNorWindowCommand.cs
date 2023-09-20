// 创建时间：2023-09-04-16:50
// 修改时间：2023-09-19-14:01

#region

#endregion

#pragma warning disable CS0067
namespace LiveBoost.Toolkit.Data;

public class MaxOrNorWindowCommand : ICommand
{
    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        if ( parameter is not DependencyObject dependencyObject || Window.GetWindow(dependencyObject) is not { } window )
        {
            return;
        }
        window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    public event EventHandler? CanExecuteChanged;
}

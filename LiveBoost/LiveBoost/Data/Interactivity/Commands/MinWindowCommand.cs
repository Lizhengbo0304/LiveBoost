// 创建时间：2023-09-04-16:50
// 修改时间：2023-09-05-17:59

#region

#endregion

#pragma warning disable CS0067

namespace LiveBoost.Data;

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

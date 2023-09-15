// 创建时间：2023-09-04-16:50
// 修改时间：2023-09-15-15:41

#pragma warning disable CS0067

namespace LiveBoost.Toolkit.Data;

public class CloseWindowCommand : ICommand
{
    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        if ( parameter is DependencyObject dependencyObject && Window.GetWindow(dependencyObject) is { } window )
        {
            window.Close();
        }
    }

    public event EventHandler? CanExecuteChanged;
}

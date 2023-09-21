// 创建时间：2023-09-15-9:05
// 修改时间：2023-09-19-14:01

using WpfScreenHelper;

namespace LiveBoost.Views;

public partial class JggMainWindow
{
    public JggMainWindow(List<RecordChannel>? channels)
    {
        InitializeComponent();
        DataContext = new JggMainWindowVm(channels);
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var screen = Screen.FromWindow(this);
        this.MaxHeight = this.WindowState == WindowState.Maximized ? screen.WorkingArea.Height : double.PositiveInfinity;
    }

    private void JggMainWindow_OnStateChanged(object sender, EventArgs e)
    {
        var screen = Screen.FromWindow(this);
        this.MaxHeight = this.WindowState == WindowState.Maximized ? screen.WorkingArea.Height : double.PositiveInfinity;
    }
}

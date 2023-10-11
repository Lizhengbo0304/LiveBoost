// 创建时间：2023-09-15-9:05
// 修改时间：2023-10-11-10:59

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
        WindowState = WindowState.Maximized;
    }
}

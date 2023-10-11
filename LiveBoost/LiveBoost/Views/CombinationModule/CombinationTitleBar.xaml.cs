// 创建时间：2023-09-05-11:48
// 修改时间：2023-10-11-10:59

namespace LiveBoost.Views;

public partial class CombinationTitleBar
{
    public CombinationTitleBar()
    {
        InitializeComponent();
    }

    private void OpenSetting_OnClick(object sender, RoutedEventArgs e)
    {
        if ( Window.GetWindow(this) is not { } window )
        {
            return;
        }
        var setting = new CombinationSettingWindow
            {Owner = window};
        setting.ShowDialog();
    }
}

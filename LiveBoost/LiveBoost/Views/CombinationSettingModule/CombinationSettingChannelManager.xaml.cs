// 创建时间：2023-09-26-10:48
// 修改时间：2023-10-13-16:16

namespace LiveBoost.Views;

public partial class CombinationSettingChannelManager
{
    public CombinationSettingChannelManager()
    {
        InitializeComponent();
    }

    private void AddChannel_OnClick(object sender, RoutedEventArgs e)
    {
        if ( Window.GetWindow(this) is not { } window )
        {
            return;
        }
        if ( CombinationSettingAddChannel.Show(window) )
        {
            SearchButton.Command?.Execute(null);
        }
    }
}

// 创建时间：2023-10-07-11:38
// 修改时间：2023-10-13-16:16

namespace LiveBoost.Views;

public partial class CombinationSettingTaskManager
{
    public CombinationSettingTaskManager()
    {
        InitializeComponent();
    }

    private void AddTask_OnClick(object sender, RoutedEventArgs e)
    {
        if ( Window.GetWindow(this) is not { } window )
        {
            return;
        }
        if ( CombinationSettingAddMission.Show(window) )
        {
            SearchButton.Command?.Execute(null);
        }
    }
}

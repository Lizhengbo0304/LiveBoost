﻿// 创建时间：2023-10-07-11:38
// 修改时间：2023-10-07-11:38

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
        if ( CombinationSettingAddChannel.Show(window) )
        {
            SearchButton.Command?.Execute(null);
        }
    }
}
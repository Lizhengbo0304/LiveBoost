﻿// 创建时间：2023-09-04-17:58
// 修改时间：2023-10-13-16:16

namespace LiveBoost.Views;

public partial class CombinationMainWindow
{
    public CombinationMainWindow()
    {
        InitializeComponent();
        DataContext = new CombinationMainWindowVm(PlayerPage.Part_ffPlay_Active, PlayerPage.SimplePanel);
    }
}

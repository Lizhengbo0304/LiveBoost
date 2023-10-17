// 创建时间：2023-09-26-9:23
// 修改时间：2023-10-13-16:16

#region

using Prism.Regions;

#endregion

namespace LiveBoost.Views;

public partial class CombinationSettingWindow
{
    public CombinationSettingWindow()
    {
        InitializeComponent();
        // 通过容器定位器获取区域管理器并创建一个新的区域管理器实例
        var regionManager = ContainerLocator.Container.Resolve<IRegionManager>().CreateRegionManager();

        // 将当前窗口与区域管理器关联
        RegionManager.SetRegionManager(this, regionManager);

        // 检查是否已经注册了名为 "SettingShow" 的区域，如果没有则进行注册
        if ( !regionManager.Regions.ContainsRegionWithName("SettingShow") )
        {
            // 注册名为 "SettingShow" 的区域，并指定视图类型为 ContentControl
            regionManager.RegisterViewWithRegion("SettingShow", typeof(ContentControl));
        }

        // 更新区域管理器中的区域信息
        RegionManager.UpdateRegions();

        Closed += (_, _) =>
        {
            // 窗口关闭时执行以下操作
            if ( regionManager.Regions.ContainsRegionWithName("SettingShow") )
            {
                // 移除名为 "SettingShow" 的区域中的所有视图
                regionManager.Regions.Remove("SettingShow");
            }

            // 清理窗口与区域管理器的关联
            RegionManager.SetRegionManager(this, null);
        };

        ContentRendered += (_, _) =>
        {
            // 在窗口渲染后执行以下操作
            // 导航到名为 "SettingShow" 的区域，显示名为 "CombinationSettingChannelManager" 的视图
            regionManager.RequestNavigate("SettingShow", "CombinationSettingChannelManager");
        };
    }
}

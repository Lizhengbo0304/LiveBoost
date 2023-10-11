// 创建时间：2023-09-04-17:58
// 修改时间：2023-10-11-10:59

namespace LiveBoost.Views;

public partial class CombinationMainWindow
{
    public CombinationMainWindow()
    {
        InitializeComponent();
        DataContext = new CombinationMainWindowVm(PlayerPage.Part_ffPlay_Active, PlayerPage.SimplePanel);
    }
}

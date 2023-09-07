// 创建时间：2023-09-04-17:58
// 修改时间：2023-09-06-9:03

namespace LiveBoost.Views;

public partial class CombinationMainWindow
{
    public CombinationMainWindow()
    {
        InitializeComponent();
        this.DataContext = new CombinationMainWindowVm(PlayerPage.Part_ffPlay_Active,PlayerPage.SimplePanel);
    }
}

// 创建时间：2023-09-07-10:38
// 修改时间：2023-09-07-10:38

namespace LiveBoost.Views;

public partial class CombinationMediaPlayerPage
{
    public CombinationMediaPlayerPage()
    {
        InitializeComponent();
    }

    private async void Slider_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if ( this.DataContext is CombinationMainWindowVm vm )
        {
            await vm.MdElement.Pause();
        }
    }
}

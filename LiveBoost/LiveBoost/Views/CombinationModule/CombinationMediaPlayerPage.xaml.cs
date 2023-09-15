// 创建时间：2023-09-07-10:38
// 修改时间：2023-09-15-15:41

namespace LiveBoost.Views;

public partial class CombinationMediaPlayerPage
{
    public CombinationMediaPlayerPage()
    {
        InitializeComponent();
        NormalSlider.AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(Slider_OnMouseLeftButtonDown), true);
        PlayListSlider.AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(Slider_OnMouseLeftButtonDown), true);
    }

    private void Slider_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if ( DataContext is CombinationMainWindowVm vm )
        {
            vm.MdElement.SeekingEnded += MdElementOnSeekingEnded;
        }
    }

    private async void MdElementOnSeekingEnded(object sender, EventArgs e)
    {
        if ( DataContext is not CombinationMainWindowVm vm )
        {
            return;
        }
        vm.MdElement.SeekingEnded -= MdElementOnSeekingEnded;
        await vm.MdElement.Pause();
    }
}

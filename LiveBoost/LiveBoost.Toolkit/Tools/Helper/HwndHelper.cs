// 创建时间：2023-09-06-14:11
// 修改时间：2023-10-13-16:17

#region

using System.Windows.Interop;

#endregion

namespace LiveBoost.Toolkit.Tools;

public static class HwndHelper
{
    public static int ViewToHwnd(this FrameworkElement element)
    {
        var p = new HwndSourceParameters
        {
            ParentWindow = new IntPtr(-3),
            WindowStyle = 1073741824
        };
        var hwndSource = new HwndSource(p)
        {
            RootVisual = element,
            SizeToContent = SizeToContent.Manual
        };
        if ( hwndSource.CompositionTarget != null )
        {
            hwndSource.CompositionTarget.BackgroundColor = Colors.Transparent;
        }
        return hwndSource.Handle.ToInt32();
    }
}

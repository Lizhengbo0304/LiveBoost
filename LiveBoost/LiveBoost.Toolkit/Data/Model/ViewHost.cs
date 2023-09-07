// 创建时间：2023-06-07-16:54
// 修改时间：2023-07-18-9:37

#region

using System.Runtime.InteropServices;
using System.Windows.Interop;

#endregion

namespace LiveBoost.Toolkit.Data;

public class ViewHost : HwndHost
{
    private readonly IntPtr _handle;

    public ViewHost(IntPtr handle) => _handle = handle;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SetParent(HandleRef hWnd, HandleRef hWndParent);

    protected override HandleRef BuildWindowCore(HandleRef hwndParent)
    {
        SetParent(new HandleRef(null, _handle), hwndParent);
        return new HandleRef(this, _handle);
    }

    protected override void DestroyWindowCore(HandleRef hwnd) { }
}

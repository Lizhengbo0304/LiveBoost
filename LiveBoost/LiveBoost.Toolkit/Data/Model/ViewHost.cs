// 创建时间：2023-09-06-14:11
// 修改时间：2023-10-11-11:00

#region

using System.Runtime.InteropServices;
using System.Windows.Interop;

#endregion

namespace LiveBoost.Toolkit.Data;

public class ViewHost : HwndHost
{
    private readonly IntPtr _handle;

    // 构造函数，接收一个窗口句柄作为参数
    public ViewHost(IntPtr handle) => _handle = handle;

    // 导入user32.dll中的SetParent函数，用于设置窗口的父窗口
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SetParent(HandleRef hWnd, HandleRef hWndParent);

    // 重写BuildWindowCore方法，用于创建窗口并设置父窗口
    protected override HandleRef BuildWindowCore(HandleRef hwndParent)
    {
        // 调用SetParent函数，将_handle指定的窗口设置为hwndParent的子窗口
        SetParent(new HandleRef(null, _handle), hwndParent);
        return new HandleRef(this, _handle);
    }

    // 重写DestroyWindowCore方法，用于销毁窗口
    protected override void DestroyWindowCore(HandleRef hwnd) { }
}

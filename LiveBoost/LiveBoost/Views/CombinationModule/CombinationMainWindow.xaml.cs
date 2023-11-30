// 创建时间：2023-09-04 17:58
// 修改时间：2023-11-07 14:12

namespace LiveBoost.Views;

public partial class CombinationMainWindow
{
    private readonly CombinationMainWindowVm _vm;

    public CombinationMainWindow()
    {
        // 初始化窗口组件
        InitializeComponent();

        // 创建 _vm 对象，传入参数
        _vm = new CombinationMainWindowVm(PlayerPage.Part_ffPlay_Active, PlayerPage.SimplePanel);

        // 设置窗口的数据上下文为 _vm
        DataContext = _vm;
    }

    private async void CombinationMainWindow_OnClosing(object sender, CancelEventArgs e)
    {
        // 检查是否有未停止的推流通道
        if (_vm.PlayAccesses?.FindAll(it => !it.Status) is not { Count: > 0 } pushAccesses)
        {
            return;
        }

        // 弹出确认对话框，询问用户是否关闭程序
        if (MessageBox.Ask($"有{pushAccesses.Count}个通道正在推流，关闭程序会立即停止推流，是否确定关闭", "关闭") is not MessageBoxResult.OK)
        {
            return;
        }

        // 取消窗口关闭
        e.Cancel = true;

        // 停止推流通道并关闭窗口
        foreach (var pushAccess in pushAccesses)
        {
            await pushAccess.PlayListPush("stop").ConfigureAwait(true);
        }

        Close();
    }
}
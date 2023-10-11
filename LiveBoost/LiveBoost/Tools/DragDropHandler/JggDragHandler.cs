// 创建时间：2023-09-19-14:22
// 修改时间：2023-10-11-10:59

#region

using GongSolutions.Wpf.DragDrop;
using LiveBoost.ToolKit.Tools;

#endregion

namespace LiveBoost.Tools;

public class JggDragHandler : IDragSource
{
    public void StartDrag(IDragInfo dragInfo)
    {
        if ( dragInfo.SourceItems.ToList<RecordChannel>() is not {Count: 1} channels )
        {
            return;
        }
        // 设置拖动数据为第一个通道
        dragInfo.Data = channels.First();
        // 设置拖放效果为所有可用效果
        dragInfo.Effects = DragDropEffects.All;
        // 发布拖动事件
        GlobalEvent.Instance.GetEvent<SourceMonitorDragEvent>().Publish(true);
    }

// 检查是否可以启动拖动操作
    public bool CanStartDrag(IDragInfo dragInfo)
    {
        // 使用 HitTestUtilities.HitTest4Type 方法检查是否点击在 ListViewItem 上
        if ( !HitTestUtilities.HitTest4Type<ListViewItem>(dragInfo.VisualSource, dragInfo.DragStartPosition) )
        {
            return false;
        }
        // 检查源项是否只有一个，以确定是否可以启动拖动
        return dragInfo.SourceItems.ToList<RecordChannel>() is {Count: 1};
    }

    public void Dropped(IDropInfo dropInfo)
    {
        // 在这里处理拖放完成后的操作
        // 可根据需要添加逻辑
    }

    public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
    {
        // 拖放操作完成后，发布拖动事件，通知其他组件
        GlobalEvent.Instance.GetEvent<SourceMonitorDragEvent>().Publish(false);
    }

    public void DragCancelled()
    {
        // 在取消拖动时执行的操作
        // 可根据需要添加逻辑
    }

    public bool TryCatchOccurredException(Exception exception) => true;
}

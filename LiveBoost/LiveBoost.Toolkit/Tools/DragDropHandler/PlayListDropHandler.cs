// 创建时间：2023-09-07-10:33
// 修改时间：2023-10-13-16:17

#region

using GongSolutions.Wpf.DragDrop;

#endregion

namespace LiveBoost.Toolkit.Tools;

public class PlayListDropHandler : IDropTarget
{
    public void DragEnter(IDropInfo dropInfo) { }


    /// <summary>
    ///     当拖拽在目标区域移动时触发的方法
    /// </summary>
    public void DragOver(IDropInfo dropInfo)
    {
        // 检查拖拽信息和目标区域是否有效
        if ( dropInfo.DragInfo is null || dropInfo.VisualTarget is not ListView {DataContext: PushAccess {IsEdit: false} pushAccess} )
        {
            dropInfo.Effects = DragDropEffects.None;
            return;
        }
        if ( TypeUtilities.GetCommonBaseClass(dropInfo.DragInfo.SourceItems) == typeof(RecordChannel) )
        {
            dropInfo.Effects = DragDropEffects.None;
            return;
        }
        // 未推流状态下允许拖拽
        if ( pushAccess.Status )
        {
            dropInfo.Effects = DragDropEffects.All;
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
        }
        else
        {
            // 推流状态下，当前播放和下一条不允许拖拽
            if ( pushAccess.CurrentIndex + 1 >= dropInfo.UnfilteredInsertIndex )
            {
                dropInfo.Effects = DragDropEffects.None;
                return;
            }

            // 如果当前播放的下一条不足20秒，则不允许拖拽
            var playLength = CalculatePlayLength(pushAccess, dropInfo.UnfilteredInsertIndex);
            if ( playLength < 20 )
            {
                dropInfo.Effects = DragDropEffects.None;
            }
            else
            {
                dropInfo.Effects = DragDropEffects.All;
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
        }
    }

    public void DragLeave(IDropInfo dropInfo) { }

    /// <summary>
    ///     当拖拽放下时触发的方法
    /// </summary>
    public void Drop(IDropInfo dropInfo)
    {
        // 检查目标区域和目标集合是否有效
        if ( dropInfo.VisualTarget is not ListView || dropInfo.TargetCollection is not ObservableList<RecordFile> targetCollection )
        {
            return;
        }
        // 检查拖拽的数据是否有效
        if ( dropInfo.Data is not List<RecordFile> files )
        {
            return;
        }

        // 根据拖拽信息的类型进行插入或移动操作
        if ( dropInfo.DragInfo is null || dropInfo.DragInfo.VisualSource != dropInfo.VisualTarget )
        {
            // 插入文件
            InsertFiles(targetCollection, files, dropInfo.UnfilteredInsertIndex);
        }
        else
        {
            // 移动文件
            MoveFiles(targetCollection, files, dropInfo.UnfilteredInsertIndex);
        }
    }

    /// <summary>
    ///     计算播放长度
    /// </summary>
    private static double CalculatePlayLength(PushAccess pushAccess, int insertIndex)
    {
        double playLength = 0;
        var index = pushAccess.CurrentIndex + 1;

        while ( index < insertIndex )
        {
            var program = pushAccess.RecordFiles[index];
            playLength += program.Duration.Value.TotalSeconds;
            if ( playLength > 20 )
            {
                break;
            }
            index++;
        }

        return playLength;
    }
    /// <summary>
    ///     插入文件到目标集合
    /// </summary>
    private void InsertFiles(ObservableList<RecordFile> targetCollection, List<RecordFile> files, int insertIndex)
    {
        foreach ( var file in files )
        {
            targetCollection.InsertItem(insertIndex, file.Clone());
        }
    }

    /// <summary>
    ///     移动文件到目标位置
    /// </summary>
    private void MoveFiles(ObservableList<RecordFile> targetCollection, List<RecordFile> files, int insertIndex)
    {
        foreach ( var file in files )
        {
            targetCollection.Move(file, insertIndex);
        }
    }
}

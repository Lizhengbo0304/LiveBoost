// 创建时间：2023-09-07-10:33
// 修改时间：2023-10-11-11:00

#region

using GongSolutions.Wpf.DragDrop;

#endregion

namespace LiveBoost.Toolkit.Tools;

public class PlayListDragHandler : IDragSource
{
    public void StartDrag(IDragInfo dragInfo)
    {
        var type = TypeUtilities.GetCommonBaseClass(dragInfo.SourceItems);
        if ( type == typeof(RecordMark) )
        {
            dragInfo.Data = dragInfo.SourceItems.OfType<RecordMark>().Select<RecordMark, RecordFile?>(mark => mark)
                .ToList();
        }
        if ( type == typeof(RecordFile) )
        {
            dragInfo.Data = dragInfo.SourceItems.OfType<RecordFile>().ToList();
        }

        dragInfo.Effects = DragDropEffects.All;
    }

    public bool CanStartDrag(IDragInfo dragInfo)
    {
        var index = dragInfo.SourceIndex;
        if ( dragInfo.VisualSource is ListView {DataContext: PushAccess pushAccess} )
        {
            // 未推流、可编辑
            if ( pushAccess.Status )
            {
                return true;
            }
            // 推流中，且处于编辑状态，不允许拖拽
            if ( pushAccess.IsEdit )
            {
                return false;
            }
            // 推流中，下一条以前的都不允许拖拽
            if ( pushAccess.CurrentIndex + 1 < index )
            {
                return true;
            }

            return false;
        }

        switch ( dragInfo )
        {
            case {VisualSource: ListView, SourceItem: RecordFile {Type: 2}}:
            case {VisualSource: ListView, SourceItem: RecordMark}:
                return true;
            default:
                return false;
        }
    }

    public void Dropped(IDropInfo dropInfo) { }

    public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo) { }

    public void DragCancelled() { }

    public bool TryCatchOccurredException(Exception exception) => true;
}

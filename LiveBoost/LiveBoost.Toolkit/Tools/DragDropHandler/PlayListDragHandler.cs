// 创建时间：2023-06-06-14:34
// 修改时间：2023-07-18-9:37

#region

using GongSolutions.Wpf.DragDrop;

#endregion

namespace LiveBoost.Toolkit.Tools;

public class PlayListDragHandler : IDragSource
{
    public void StartDrag(IDragInfo dragInfo)
    {
    }

    public bool CanStartDrag(IDragInfo dragInfo)
    {
        return true;
    }

    public void Dropped(IDropInfo dropInfo) { }

    public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo) { }

    public void DragCancelled() { }

    public bool TryCatchOccurredException(Exception exception) => true;
}

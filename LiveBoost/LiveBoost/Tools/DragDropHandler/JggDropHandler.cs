// 创建时间：2023-09-19-17:35
// 修改时间：2023-10-11-10:59

#region

using GongSolutions.Wpf.DragDrop;
using LiveBoost.Controls;

#endregion

namespace LiveBoost.Tools;

public class JggDropHandler : IDropTarget
{
    public void DragEnter(IDropInfo dropInfo) { }

    public void DragOver(IDropInfo dropInfo)
    {
        if ( dropInfo.VisualTargetItem is null )
        {
            dropInfo.Effects = DragDropEffects.None;
            return;
        }
        dropInfo.Effects = DragDropEffects.All;
    }

    public void DragLeave(IDropInfo dropInfo) { }

    public void Drop(IDropInfo dropInfo)
    {
        switch ( dropInfo.VisualTargetItem )
        {
            case null:
                return;
            case JggItem jggItem when dropInfo.DragInfo.Data is RecordChannel channel:
                jggItem.Channel = channel;
                break;
        }
    }
}

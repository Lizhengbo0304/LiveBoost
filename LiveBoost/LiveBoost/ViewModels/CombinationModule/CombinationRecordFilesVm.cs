// 创建时间：2023-09-07-15:06
// 修改时间：2023-09-15-15:41

namespace LiveBoost.ViewModels;

public partial class CombinationMainWindowVm
{
#region Properties

    /// <summary>
    ///     公共素材库
    /// </summary>
    public RecordFile? PublicRecordFile { get; set; }

    /// <summary>
    ///     我的收录
    /// </summary>
    public RecordFile? MyRecordFile { get; set; }

    /// <summary>
    ///     当前素材Mark点
    /// </summary>
    public ObservableList<RecordMark>? CurrentMarks { get; set; }

    /// <summary>
    ///     拖拽事件
    /// </summary>
    public PlayListDragHandler PlayListDragHandler => new();

#endregion
#region Commands

    /// <summary>
    ///     收录文件的双击操作
    /// </summary>
    public DelegateCommand<MouseButtonEventArgs> RecordFileDoubleClickCmd { get; set; }

    public DelegateCommand<RecordFile> RefreshRecordCmd => new(async file =>
    {
        file.Children = await file.GetRecordChildren(file.SearchType);
    });

#endregion
#region Command-Event

    /// <summary>
    ///     收录文件的双击操作
    /// </summary>
    private async void RecordFileDoubleClickExecute(MouseButtonEventArgs args)
    {
        // 检查是否有有效的FrameworkElement
        if ( args.OriginalSource is not FrameworkElement frameworkElement )
        {
            return;
        }

        // 检查是否有有效的ListViewItem
        if ( frameworkElement.FindVisualParent<ListViewItem>() is not { } listViewItem )
        {
            return;
        }

        // 根据DataContext的类型进行不同的操作
        switch ( listViewItem.DataContext )
        {
            case RecordFile {Type: 1} folder:
                // 处理文件夹的双击操作
                await HandleFolderDoubleClick(folder);
                break;
            case RecordFile file:
                // 处理文件类型
                await PlayRecordFileAsync(file);
                break;
            case RecordMark mark:
                // 处理标记点类型的记录文件
                await PlayMarkAsync(mark);
                break;
        }
    }
    /// <summary>
    ///     处理文件夹的双击操作
    /// </summary>
    private async Task HandleFolderDoubleClick(RecordFile folder)
    {
        // 获取文件夹的子文件
        folder.Children = await folder.GetRecordChildren(folder.SearchType);

        // 根据SearchType设置不同的文件夹属性
        if ( folder.SearchType == 1 )
        {
            MyRecordFile = folder;
        }
        else
        {
            PublicRecordFile = folder;
        }
    }

#endregion
#region Event

    // 初始化 MyRecordFile
    private async Task InitializeMyRecordFileAsync()
    {
        MyRecordFile = new RecordFile {Name = "我的收录", SearchType = 1, Id = "0"};
        MyRecordFile.Children = await MyRecordFile.GetRecordChildren(1);
    }

// 初始化 PublicRecordFile
    private async Task InitializePublicRecordFileAsync()
    {
        PublicRecordFile = new RecordFile {Name = "公共收录", SearchType = 2, Id = "0"};
        PublicRecordFile.Children = await PublicRecordFile.GetRecordChildren(2);
    }

#endregion
}

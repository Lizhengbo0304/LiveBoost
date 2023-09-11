// 创建时间：2023-09-07-15:06
// 修改时间：2023-09-07-15:06

namespace LiveBoost.ViewModels;

public partial class CombinationMainWindowVm
{
#region Properties
    /// <summary>
    /// 公共素材库
    /// </summary>
    public RecordFile? PublicRecordFile { get; set; }

    /// <summary>
    /// 我的收录
    /// </summary>
    public RecordFile? MyRecordFile { get; set; }

    /// <summary>
    /// 当前素材Mark点
    /// </summary>
    public ObservableList<RecordMark>? CurrentMarks { get; set; }

    /// <summary>
    /// 拖拽事件
    /// </summary>
    public PlayListDragHandler PlayListDragHandler => new();
#endregion
#region Commands

/// <summary>
/// 处理记录文件的双击操作
/// </summary>
public DelegateCommand<MouseButtonEventArgs> RecordFileDoubleClickCmd => new(
    async args =>
    {
        // 检查是否有有效的FrameworkElement
        if (args.OriginalSource is not FrameworkElement frameworkElement)
        {
            return;
        }

        // 检查是否有有效的ListViewItem
        if (frameworkElement.FindVisualParent<ListViewItem>() is not { } listViewItem)
        {
            return;
        }

        // 根据DataContext的类型进行不同的操作
        switch (listViewItem.DataContext)
        {
            case RecordFile { Type: 1 } folder:
                // 处理文件夹类型的记录文件
                folder.Children = await folder.GetRecordChildren(folder.SearchType);
                if (folder.SearchType == 1)
                {
                    MyRecordFile = folder;
                }
                else
                {
                    PublicRecordFile = folder;
                }
                break;
            case RecordFile file:
                // 处理文件类型的记录文件
                if (MdElement.IsOpen)
                {
                    await MdElement.Close();
                }
                MdElement.Visibility = Visibility.Collapsed;
                MdActive.Visibility = Visibility.Visible;
                MdElement = MdActive;
                // 检查文件是否存在
                if (!File.Exists(file.FullPath))
                {
                    MessageBox.Warning("文件丢失，无法预览", "播放");
                    return;
                }

                // 关闭播放列表模式下的所有播放文件
                if (IsPlayListMode && (PlayFiles?.Any() ?? false))
                {
                    PlayFiles.ForEach(async it =>
                    {
                        if (it.MediaElement is not null)
                        {
                            await it.MediaElement.Close();
                        }
                        it.MediaElement = null;
                    });
                }
                IsPlayListMode = false;
                // 清除播放器的标记点
                RecordPlaybackFfPlayerCleanPointsCmd.Execute();

                // 设置播放文件相关的属性
                PlayName = file.Name;
                PlayMode = PlayMode.RecordFile;
                CurrentMarks = file.Markers ??= new ObservableList<RecordMark>();
                PlayFile = file;

                // 打开并播放文件
                await MdElement.Open(new Uri(file.FullPath));
                await MdElement.Play();
                break;
            case RecordMark mark:
                // 处理标记点类型的记录文件
                RecordPlaybackFfPlayerCleanPointsCmd.Execute();
                await MdElement.Pause();

                // 解析标记点的时间
                if (!TimeSpan.TryParse(mark.InPoint, out var inpoint))
                {
                    inpoint = TimeSpan.Zero;
                }
                SliderIn = inpoint.TotalMilliseconds;
                if (!TimeSpan.TryParse(mark.OutPoint, out var outpoint))
                {
                    outpoint = TimeSpan.Zero;
                }
                SliderOut = outpoint.TotalMilliseconds;
                IsInSet = true;
                IsOutSet = true;

                // 设置播放文件相关的属性
                PlayName = mark.Name;
                PlayMode = PlayMode.SubRecordFile;
                PlayMark = mark;

                // 跳转到标记点的时间
                await MdElement.Seek(inpoint);
                break;
        }
    });


    public DelegateCommand<RecordFile> RefreshRecordCmd => new(async file =>
    {
        file.Children = await file.GetRecordChildren(file.SearchType);
    });

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

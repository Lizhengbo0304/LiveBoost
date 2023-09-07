// 创建时间：2023-09-07-10:21
// 修改时间：2023-09-07-10:21

using LiveBoost.Tools;

namespace LiveBoost.ViewModels;

public sealed partial class CombinationMainWindowVm
{
#region Properties

    /// <summary>
    ///     推流通道列表
    /// </summary>
    public List<PushAccess>? PlayAccesses { get; set; }

    /// <summary>
    ///     当前播单
    /// </summary>
    public PushAccess? CurrentPlayList { get; set; }

#endregion
#region Event

    // 初始化 PlayAccesses
    private async Task InitializePlayAccessesAsync()
    {
        Debug.WriteLine("11111");
        var playTemplates = await 10.GetShouluTemplates().ConfigureAwait(false);
        PlayAccesses = await UrlHelper.GetPushAccess().ConfigureAwait(false);

        if ( PlayAccesses!.Any() )
        {
            // 处理 PlayAccesses 不为空的情况
            PlayAccesses!.First()!.IsSelected = true;
            PlayAccesses!.ForEach(async it =>
            {
                if ( playTemplates.Find(template => template.Id == it.AccessId) is { } recordTemplate )
                {
                    it.RecordFiles = recordTemplate.RecordFiles;
                }
                else
                {
                    // 创建播单
                    if ( !await UrlHelper.NewPlayList(it.AccessId!, it.Title!, it.Mode) )
                    {
                        MessageBox.Warning($"{it.Title}创建播单异常，无法存储", "推流通道");
                    }
                }

                it.RecordFiles.CollectionChanged += RecordFilesOnCollectionChanged;
            });

        }
    }

    private void RecordFilesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {

    }

#endregion
}

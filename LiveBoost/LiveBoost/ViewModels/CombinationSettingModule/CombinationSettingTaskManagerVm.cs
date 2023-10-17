// 创建时间：2023-10-08-9:49
// 修改时间：2023-10-13-16:16

#region

using LiveBoost.ToolKit.Tools;

#endregion

namespace LiveBoost.ViewModels;

public class CombinationSettingTaskManagerVm : INotifyPropertyChanged
{
    public CombinationSettingTaskManagerVm()
    {
        GetServers();
        // 初始化搜索命令
        SearchCommand = new DelegateCommand(async () =>
        {
            // 异步执行搜索操作
            CurrentPage = 1;
            var result = await SearchName.SearchRecordMissionsAsync(SelectedClientName, CurrentPage);
            result.missions.SetIndexes();
            Missions = result.missions; // 更新Mission集合
            TotalPage = result.totalPage; // 更新TotalPage
        });

        // 初始化重置命令
        ResetCommand = new DelegateCommand(() =>
        {
            // 重置搜索条件和页码，然后执行搜索命令
            SearchName = string.Empty;
            CurrentPage = 1;
            SearchCommand.Execute();
        });

        // 初始化分页更新命令
        PageUpdatedCmd = new DelegateCommand<FunctionEventArgs<int>>(async pageInfo =>
        {
            // 异步执行分页更新操作
            var result = await SearchName.SearchRecordMissionsAsync(SelectedClientName, pageInfo.Info);
            result.missions.SetIndexes();
            Missions = result.missions; // 更新Mission集合
            TotalPage = result.totalPage; // 更新TotalPage
        });

        // 删除命令
        DeleteCommand = new DelegateCommand<RecordMission>(async channel =>
        {
            // 弹出确认删除的对话框，如果用户不确认删除，则不执行后续操作
            if ( MessageBox.Ask("是否确定删除任务", "删除") is not MessageBoxResult.OK )
            {
                return;
            }

            // 调用频道的删除方法，如果删除成功，执行搜索命令以刷新列表
            if ( await channel.DeleteMission().ConfigureAwait(false) )
            {
                SearchCommand.Execute();
            }
        });

// 编辑命令
        EditCommand = new DelegateCommand<RecordMission>(mission =>
        {
            // 检查主窗口是否存在子窗口，如果没有子窗口，则不执行编辑操作
            if ( !( AppProgram.Instance.App.MainWindow?.OwnedWindows.Count > 0 ) )
            {
                return;
            }

            // 弹出编辑窗口并传递当前频道信息，编辑完成后执行搜索命令以刷新列表
            CombinationSettingAddMission.Show(AppProgram.Instance.App.MainWindow?.OwnedWindows[0]!, mission);
            SearchCommand.Execute();
            SearchCommand.Execute();
        });

// 编辑状态命令
        EditStatusCommand = new DelegateCommand<RecordMission>(async channel =>
        {
            // 调用编辑频道状态的方法，如果编辑失败，则还原频道状态
            if ( !await channel.EditMissionStatus().ConfigureAwait(false) )
            {
                channel.Status = !channel.Status;
            }
        });

        // 执行初始搜索
        SearchCommand.Execute();
    }
#region Events

    public async void GetServers()
    {
        Servers = await UrlHelper.GetShouluServersAll().ConfigureAwait(false);
    }

#endregion
#region INotifyPropertyChangedEvent

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        switch ( propertyName )
        {
            case nameof(SelectedClientName):
                SearchCommand.Execute();
                break;
        }
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if ( EqualityComparer<T>.Default.Equals(field, value) )
        {
            return false;
        }
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

#endregion

#region Properties

    /// <summary>
    ///     搜索关键词
    /// </summary>
    public string SearchName { get; set; } = string.Empty;

    /// <summary>
    ///     服务器列表
    /// </summary>
    public List<RecordServer> Servers { get; set; } = new();
    /// <summary>
    ///     选中的服务器名称
    /// </summary>
    public string? SelectedClientName { get; set; }

    /// <summary>
    ///     任务列表
    /// </summary>
    public List<RecordMission> Missions { get; set; } = new();
    /// <summary>
    ///     频道总页数
    /// </summary>
    public int TotalPage { get; set; }

    /// <summary>
    ///     当前页数
    /// </summary>
    public int CurrentPage { get; set; } = 1;

#endregion
#region Commands

    /// <summary>
    ///     编辑状态频道
    /// </summary>
    public DelegateCommand<RecordMission> EditStatusCommand { get; set; }
    /// <summary>
    ///     搜索命令
    /// </summary>
    public DelegateCommand SearchCommand { get; }
    /// <summary>
    ///     重置命令
    /// </summary>
    public DelegateCommand ResetCommand { get; }

    /// <summary>
    ///     收录通道翻页命令
    /// </summary>
    public DelegateCommand<FunctionEventArgs<int>> PageUpdatedCmd { get; }

    /// <summary>
    ///     删除频道
    /// </summary>
    public DelegateCommand<RecordMission> DeleteCommand { get; set; }
    /// <summary>
    ///     编辑频道
    /// </summary>
    public DelegateCommand<RecordMission> EditCommand { get; set; }

#endregion
}

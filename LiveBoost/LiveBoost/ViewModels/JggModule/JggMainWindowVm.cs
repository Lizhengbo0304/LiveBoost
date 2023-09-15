﻿// 创建时间：2023-09-15-14:49
// 修改时间：2023-09-15-15:41

namespace LiveBoost.ViewModels;

public partial class JggMainWindowVm : INotifyPropertyChanged
{
#region Ctor

    public JggMainWindowVm()
    {
        // 初始化收录
        Task.Run(async () =>
        {
            //    初始化收录通道
            await InitializeRecordChannelsAsync();
        });
    }

#endregion
#region INotifyPropertyChangedEvent

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
}

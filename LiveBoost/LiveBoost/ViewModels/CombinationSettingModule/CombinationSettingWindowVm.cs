// 创建时间：2023-09-26-14:20
// 修改时间：2023-10-13-16:16

#region

using Prism.Regions;

#endregion

namespace LiveBoost.ViewModels;

public class CombinationSettingWindowVm : INotifyPropertyChanged
{
#region Ctor

    public CombinationSettingWindowVm()
    {
        ChangeViewCommand = new DelegateCommand<RadioButton>(button =>
        {
            if ( Window.GetWindow(button) is not { } settingWindow )
            {
                return;
            }
            if ( RegionManager.GetRegionManager(settingWindow) is not { } regionManager )
            {
                return;
            }
            regionManager.RequestNavigate("SettingShow", button.Tag as string);
        });
    }

#endregion
#region Command

    /// <summary>
    ///     更改管理界面
    /// </summary>
    public DelegateCommand<RadioButton> ChangeViewCommand { get; set; }

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

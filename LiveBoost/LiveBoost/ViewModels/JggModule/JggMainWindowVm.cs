// 创建时间：2023-09-15-14:49
// 修改时间：2023-10-11-10:59

#region

using LiveBoost.Controls;

#endregion

namespace LiveBoost.ViewModels;

public partial class JggMainWindowVm : INotifyPropertyChanged
{
#region Ctor

    public JggMainWindowVm(List<RecordChannel>? channels)
    {
        RecordChannels = channels;
        // 初始化收录
        JggDragHandler = new JggDragHandler();
        JggDropHandler = new JggDropHandler();

        RecordItems = new List<JggItem>
        {
            new(),
            new(),
            new(),
            new(),
            new(),
            new(),
            new(),
            new(),
            new(),
            new(),
            new(),
            new(),
            new(),
            new(),
            new(),
            new()
        };

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

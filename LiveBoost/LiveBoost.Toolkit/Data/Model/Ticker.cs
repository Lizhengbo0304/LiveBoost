// 创建时间：2023-09-04-16:09
// 修改时间：2023-09-06-9:03

namespace LiveBoost.Toolkit.Data;

public sealed class Ticker : INotifyPropertyChanged
{
    public DateTime Now => DateTime.Now;
    public DateTime Today => DateTime.Today;
    public string AmOrPm => Now.Hour >= 12 ? "PM" : "AM";

#region INotifyPropertyChangedEvent

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
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

#region Ctor

    private static readonly Lazy<Ticker> TickerLazy = new(() => new Ticker());

    private Ticker()
    {
        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(10) // 0.01 second updates
        };
        timer.Tick += (_, _) =>
        {
            OnPropertyChanged(nameof(Now));
            OnPropertyChanged(nameof(Today));
        };
        timer.Start();
    }
    public static Ticker Instance => TickerLazy.Value;

#endregion
}

// 创建时间：2023-09-04-16:09
// 修改时间：2023-10-13-16:17

namespace LiveBoost.Toolkit.Data;

public sealed class Ticker : INotifyPropertyChanged
{
    // 当前时间
    public DateTime Now => DateTime.Now;

    // 当天日期
    public DateTime Today => DateTime.Today;

    // 上午或下午
    public string AmOrPm => Now.Hour >= 12 ? "PM" : "AM";

    #region INotifyPropertyChangedEvent

    // 属性更改事件
    public event PropertyChangedEventHandler? PropertyChanged;

    // 触发属性更改事件
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // 设置字段的值，并触发属性更改事件
    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    #endregion

    #region Ctor

    // 使用Lazy<T>实现延迟加载的单例模式
    private static readonly Lazy<Ticker> TickerLazy = new(() => new Ticker());

    // 私有构造函数
    private Ticker()
    {
        // 创建一个DispatcherTimer定时器，每0.01秒更新一次时间
        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(10) // 0.01 second updates
        };
        timer.Tick += (_, _) =>
        {
            // 更新Now和Today属性，并触发属性更改事件
            OnPropertyChanged(nameof(Now));
            OnPropertyChanged(nameof(Today));
        };
        timer.Start();
    }

    // 获取Ticker的单例实例
    public static Ticker Instance => TickerLazy.Value;

    #endregion
}
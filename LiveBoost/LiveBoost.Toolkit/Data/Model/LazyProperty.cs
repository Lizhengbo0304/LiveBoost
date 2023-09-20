// 创建时间：2023-09-04-16:09
// 修改时间：2023-09-19-14:01

namespace LiveBoost.Toolkit.Data;

public sealed class LazyProperty<TD> : INotifyPropertyChanged
{
#region Ctor

    public LazyProperty(Func<CancellationToken, Task<TD>> retrievalFunc, TD? defaultValue)
    {
        _retrievalFunc = retrievalFunc ?? throw new ArgumentNullException(nameof(retrievalFunc));
        _defaultValue = defaultValue;

        _value = default;
    }

#endregion

    private async Task<TD?> LoadValueAsync() => await _retrievalFunc(_cancelTokenSource.Token);

    public void CancelLoading()
    {
        _cancelTokenSource.Cancel();
    }

    /// <summary> This allows you to assign the value of this lazy property directly to a variable of type T </summary>
    public static implicit operator TD?(LazyProperty<TD?> p) => p.Value;

#region Field

    private TD? _value;
    private readonly CancellationTokenSource _cancelTokenSource = new();

    private readonly TD? _defaultValue;

    private readonly Func<CancellationToken, Task<TD>> _retrievalFunc;

#endregion

#region Property

    private bool IsLoaded { get; set; }

    public bool IsLoading { get; set; }

    public bool ErrorOnLoading { get; set; }

    public TD? Value
    {
        get
        {
            if ( IsLoaded )
            {
                return _value;
            }

            if ( IsLoading )
            {
                return _defaultValue;
            }
            IsLoading = true;
            LoadValueAsync()
                .ContinueWith(t =>
                {
                    if ( t.IsCanceled )
                    {
                        return;
                    }
                    if ( t.IsFaulted )
                    {
                        _value = _defaultValue;
                        ErrorOnLoading = true;
                        IsLoaded = true;
                        IsLoading = false;
                        OnPropertyChanged();
                    }
                    else
                    {
                        Value = t.Result;
                    }
                });

            return _defaultValue;
        }
        // if you want a ReadOnly-property just set this setter to private
        set
        {
            if ( IsLoading )
                // since we set the value now, there is no need
                // to retrieve the "old" value asynchronously
            {
                CancelLoading();
            }

            if ( value != null && _value != null && EqualityComparer<TD>.Default.Equals(_value, value) )
            {
                return;
            }
            _value = value;
            IsLoaded = true;
            IsLoading = false;
            ErrorOnLoading = false;

            OnPropertyChanged();
        }
    }

#endregion

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
}

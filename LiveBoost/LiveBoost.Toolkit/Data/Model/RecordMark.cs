// 创建时间：2023-09-07-10:33
// 修改时间：2023-09-07-10:33

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LiveBoost.Toolkit.Data;

public class RecordMark : INotifyPropertyChanged
{
#region INotifyPropertyChangedEvent

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if ( EqualityComparer<T>.Default.Equals(field, value) ) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

#endregion

    
}


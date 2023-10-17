// 创建时间：2023-09-05-14:25
// 修改时间：2023-10-13-16:17

namespace LiveBoost.Toolkit.Data;

public sealed class ObservableList<T> : List<T>, INotifyCollectionChanged
{
    public ObservableList() { }

    public ObservableList(int capacity) : base(capacity) { }


    public ObservableList(List<T>? list)
    {
        if ( list is {Count: > 0} )
        {
            AddRange(list);
        }
    }
    private List<T>? BackupList { get; set; }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    ///     恢复到上一次的备份
    /// </summary>
    public void BackupRecover()
    {
        Clear();
        if ( BackupList != null )
        {
            AddRange(BackupList);
        }
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    ///     添加数据
    /// </summary>
    public void AddItem(T t)
    {
        Backup();
        Add(t);
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, t));
    }
    /// <summary>
    ///     请空列表
    /// </summary>
    public void ClearList()
    {
        Backup();
        Clear();
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    ///     插入数据
    /// </summary>
    public void InsertItem(int index, T t)
    {
        Backup();
        Insert(index, t);
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, t));
    }

    /// <summary>
    ///     添加一个列表
    /// </summary>
    public void AddItemRange(List<T>? list)
    {
        if ( list is not {Count: > 0} )
        {
            return;
        }
        Backup();
        AddRange(list);
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    ///     通知集合改变
    /// </summary>
    public async void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if ( Dispatcher.CurrentDispatcher == Application.Current.Dispatcher )
        {
            CollectionChanged?.Invoke(this, e);
        }
        else
        {
            await Application.Current.Dispatcher.InvokeAsync(() => CollectionChanged?.Invoke(this, e));
        }
    }

    /// <summary>
    ///     删除数据
    /// </summary>
    public void RemoveItem(T t)
    {
        Backup();
        Remove(t);
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, t));
    }

    /// <summary>
    ///     根据Index删除数据
    /// </summary>
    public void RemoveItem(int index)
    {
        Backup();
        RemoveAt(index);
    }
    public void RemoveItem(Predicate<T> match)
    {
        Backup();
        var item = Find(match);
        if ( item != null )
        {
            RemoveItem(item);
        }
    }
    public void RemoveItem(T t, IEqualityComparer<T> comparer)
    {
        Backup();
        var item = Find(item => comparer.Equals(t, item));
        if ( item != null )
        {
            RemoveItem(item);
        }
    }
    public void ResetIndex(int index, T t)
    {
        if ( index < 0 || index > Count - 1 )
        {
            return;
        }

        Backup();
        this[index] = t;
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, this[index], index));
    }
    public void Move(T t, int newIndex)
    {
        var oldIndex = IndexOf(t);
        if ( oldIndex < 0 || newIndex < 0 )
        {
            return;
        }

        if ( oldIndex == newIndex )
        {
            return;
        }

        Backup();
        Insert(newIndex, t);
        RemoveAt(oldIndex > newIndex ? oldIndex + 1 : oldIndex);
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
    /// <summary>
    ///     备份当前列表
    /// </summary>
    private void Backup()
    {
        BackupList = new List<T>(this);
    }
}

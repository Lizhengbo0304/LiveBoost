// 创建时间：2023-09-04-16:09
// 修改时间：2023-10-13-16:17

namespace LiveBoost.Toolkit.Data;

public sealed class LoginUser : INotifyPropertyChanged
{
#region INotifyPropertyChanged-Event

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

#region LoginProperty

    public string? Token { get; set; }

    /// <summary>
    ///     用户ID
    /// </summary>
    [JsonProperty("id")]
    public string? Id { get; set; }

    /// <summary>
    ///     登录名称
    /// </summary>
    [JsonProperty("loginName")]
    public string? LoginName { get; set; }

    /// <summary>
    ///     用户姓名
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <summary>
    ///     角色
    /// </summary>
    [JsonProperty("roleNames")]
    public string? RoleNames { get; set; }

    public string DisplayInfo => string.IsNullOrEmpty(RoleNames) ? $"{Name} | " : $"{Name} | {RoleNames} | ";

#endregion
}

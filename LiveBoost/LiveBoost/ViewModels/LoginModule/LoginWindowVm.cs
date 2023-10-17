// 创建时间：2023-09-04-17:07
// 修改时间：2023-10-13-16:16

namespace LiveBoost.ViewModels;

public class LoginWindowVm : INotifyPropertyChanged
{
    public LoginWindowVm() => LoginCommand = new DelegateCommand(LoginCommandExecute);

#region Command

    /// <summary>
    ///     登录命令
    /// </summary>
    public DelegateCommand LoginCommand { get; }

#endregion
#region Event

    /// <summary>
    ///     登录命令
    /// </summary>
    private async void LoginCommandExecute()
    {
        if ( !AppConfig.Instance.IsInit )
        {
            MessageBox.Warning("配置文件获取异常，请检查接口配置", "登录");
            return;
        }

        // 防止用户多次点击
        IsLoginEnable = false;

        // 用户名或密码是否为空
        if ( string.IsNullOrEmpty(UserName?.Trim()) )
        {
            MessageBox.Warning("用户名不能为空", "登录");
            IsLoginEnable = true;
            GlobalEvent.Instance.GetEvent<LoginWndInputBoxFocus>().Publish(true);
            return;
        }

        if ( string.IsNullOrEmpty(Password) )
        {
            MessageBox.Warning("密码不能为空", "登录");
            IsLoginEnable = true;
            GlobalEvent.Instance.GetEvent<LoginWndInputBoxFocus>().Publish(false);
            return;
        }

        if ( await UrlHelper.Login(UserName!.Trim(), Password) )
        {
            Password = string.Empty;
            await AppProgram.Instance.LoginInit();
        }
        else
        {
            IsLoginEnable = true;
            Password = string.Empty;
            GlobalEvent.Instance.GetEvent<LoginWndInputBoxFocus>().Publish(false);
        }
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
#region Property

    /// <summary>
    ///     登录按钮以及账户名、密码输入框是否可用
    /// </summary>
    public bool IsLoginEnable { get; set; } = true;

    /// <summary>
    ///     账户名
    /// </summary>
    public string? UserName { get; set; }
#if DEBUG
    // = "admin";
        = "lizhengbo";
#endif

    /// <summary>
    ///     密码
    /// </summary>
    public string? Password { get; set; }
#if DEBUG
    // = "admin123";
        = "Mam@2022";
#endif

#endregion
}

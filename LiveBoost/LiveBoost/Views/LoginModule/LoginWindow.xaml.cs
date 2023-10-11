// 创建时间：2023-09-04-11:58
// 修改时间：2023-10-11-10:59

namespace LiveBoost.Views;

public partial class LoginWindow
{
    public LoginWindow()
    {
        InitializeComponent();
        ContentRendered += (_, _) => UserNameTextBox.Focus();
        LoginWndInputBoxFocus.SubscriptionTokens.Add(GlobalEvent.Instance.GetEvent<LoginWndInputBoxFocus>().Subscribe(isName =>
        {
            Dispatcher.Invoke(() =>
            {
                if ( isName )
                {
                    UserNameTextBox.Focus();
                }
                else
                {
                    UserPasswordBox.Focus();
                }
            });
        }));
    }

    private void UserNameTextBox_OnGotFocus(object sender, RoutedEventArgs e)
    {
        UserNameTextBox.SelectAll();
    }

    private void UserPasswordBox_OnGotFocus(object sender, RoutedEventArgs e)
    {
        UserPasswordBox.Clear();
    }
}

// 创建时间：2023-09-04-11:58
// 修改时间：2023-09-15-15:41

namespace LiveBoost.Views;

public partial class LoginWindow
{
    public LoginWindow()
    {
        InitializeComponent();
        ContentRendered += (_, _) => UserNameTextBox.Focus();

    }
}

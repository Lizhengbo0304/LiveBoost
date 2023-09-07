// 创建时间：2023-09-04-11:58
// 修改时间：2023-09-06-9:03

namespace LiveBoost.Views;

public partial class LoginWindow
{
    public LoginWindow()
    {
        InitializeComponent();
        ContentRendered += (_, _) => UserNameTextBox.Focus();
    }
}

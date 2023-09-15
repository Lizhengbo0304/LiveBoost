// 创建时间：2023-09-07-15:58
// 修改时间：2023-09-15-15:41

#region

#endregion

namespace LiveBoost.Views;

public partial class CombinationPlayListOutXml
{
#region Static-Event

    public static string? ShowOut()
    {
        var outWindow = new CombinationPlayListOutXml();
        outWindow.ShowDialog();
        return outWindow._outResult;
    }

#endregion

#region Private Event

    private string? _outResult;

    private CombinationPlayListOutXml()
    {
        InitializeComponent();
        Owner = AppProgram.Instance.MainWnd;
    }

    private void Done_OnClick(object sender, RoutedEventArgs e)
    {
        if ( string.IsNullOrEmpty(OutName) )
        {
            MessageBox.Warning("文件名称不得为空", Title);
            return;
        }
        if ( OutName.HasInvalidSymbol() )
        {
            MessageBox.Warning("文件名称包含非法字符，请重新输入", Title);
            return;
        }

        _outResult = OutName;
        Close();
    }

#endregion
#region depe

    /// <summary>
    ///     文件名称
    /// </summary>
    public static readonly DependencyProperty OutNameProperty = DependencyProperty.Register(
        nameof(OutName), typeof(string), typeof(CombinationPlayListOutXml), new PropertyMetadata(default(string)));

    /// <summary>
    ///     文件名称
    /// </summary>
    public string OutName
    {
        get => (string) GetValue(OutNameProperty);
        set => SetValue(OutNameProperty, value);
    }

#endregion
}

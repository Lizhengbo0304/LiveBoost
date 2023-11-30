// 创建时间：2023-09-07-15:58
// 修改时间：2023-10-13-16:16

#region

using LiveBoost.ToolKit.Data;

#endregion

namespace LiveBoost.Views;

public partial class CombinationPlayListOut
{
    #region Static-Event

    public static (string, RecordTemplate) ShowOut()
    {
        var outWindow = new CombinationPlayListOut();
        outWindow.ShowDialog();
        return outWindow._outResult;
    }

    #endregion

    #region Private Event

    private (string, RecordTemplate) _outResult;

    private CombinationPlayListOut()
    {
        InitializeComponent();
        Owner = AppProgram.Instance.MainWnd;
        Task.Run(() => { Dispatcher.Invoke(async () => { OutTemplates = await 2.GetShouluTemplates(); }); });
    }

    private void Done_OnClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(OutName))
        {
            MessageBox.Warning("文件名称不得为空", "导出视频");
            return;
        }

        if (OutName.HasInvalidSymbol())
        {
            MessageBox.Warning("文件名称包含非法字符，请重新输入", "导出视频");
            return;
        }

        if (SelectedTemplate is null)
        {
            MessageBox.Warning("请选择导出模板", "导出视频");
            return;
        }

        if (string.IsNullOrEmpty(SelectedTemplate.Id))
        {
            MessageBox.Warning("导出模板Id为空，请重新选择", "导出播单");

            return;
        }

        _outResult = (OutName, SelectedTemplate);
        Close();
    }

    #endregion

    #region depe

    /// <summary>
    ///     导出模板列表
    /// </summary>
    public static readonly DependencyProperty OutTemplatesProperty = DependencyProperty.Register(
        nameof(OutTemplates), typeof(List<RecordTemplate>), typeof(CombinationPlayListOut),
        new PropertyMetadata(default(List<RecordTemplate>)));

    /// <summary>
    ///     导出模板列表
    /// </summary>
    public List<RecordTemplate> OutTemplates
    {
        get => (List<RecordTemplate>)GetValue(OutTemplatesProperty);
        set => SetValue(OutTemplatesProperty, value);
    }

    /// <summary>
    ///     当前选择模板
    /// </summary>
    public static readonly DependencyProperty SelectedTemplateProperty = DependencyProperty.Register(
        nameof(SelectedTemplate), typeof(RecordTemplate), typeof(CombinationPlayListOut),
        new PropertyMetadata(default(RecordTemplate)));

    /// <summary>
    ///     当前选择模板
    /// </summary>
    public RecordTemplate? SelectedTemplate
    {
        get => (RecordTemplate?)GetValue(SelectedTemplateProperty);
        set => SetValue(SelectedTemplateProperty, value);
    }

    /// <summary>
    ///     文件名称
    /// </summary>
    public static readonly DependencyProperty OutNameProperty = DependencyProperty.Register(
        nameof(OutName), typeof(string), typeof(CombinationPlayListOut), new PropertyMetadata(default(string)));

    /// <summary>
    ///     文件名称
    /// </summary>
    public string OutName
    {
        get => (string)GetValue(OutNameProperty);
        set => SetValue(OutNameProperty, value);
    }

    #endregion
}
// 创建时间：2023-10-08-16:43
// 修改时间：2023-10-13-16:17

#region

using System.Windows.Data;

#endregion

namespace LiveBoost.Toolkit.Controls;

// 使用TemplatePart属性来定义控件模板中的部件
[TemplatePart(Name = "Part_MainGrid", Type = typeof(Grid))]
public class TextShow : Control
{
    #region Property

    private Grid? MainGrid;

    #endregion

    public TextShow()
    {
        // 当控件大小改变时，重置网格
        SizeChanged += (_, _) => { ResetGrid(); };
    }

    // 当依赖属性改变时，重置网格
    private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextShow { MainGrid: not null } textShow)
        {
            textShow.ResetGrid();
        }
    }

    /// <summary>
    ///     重置网格，清空所有列和子元素，然后根据当前文本重新创建列和文本块
    /// </summary>
    private void ResetGrid()
    {
        if (MainGrid is null)
        {
            return;
        }

        MainGrid.Children.Clear();
        MainGrid.ColumnDefinitions.Clear();
        if (string.IsNullOrEmpty(Text))
        {
            return;
        }

        for (var i = 0; i < (Text.Length * 2 - 1); i++)
        {
            if ((i % 2) == 1)
            {
                // 添加一个宽度为1的列，用于在字符之间添加空白
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }
            else
            {
                // 添加一个自动宽度的列，用于放置文本块
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                // 创建和配置文本块
                var textBlock = CreateTextBlock(i);
                MainGrid.Children.Add(textBlock);
                Grid.SetColumn(textBlock, MainGrid.ColumnDefinitions.Count - 1);
            }
        }
    }

    /// <summary>
    ///     创建和配置文本块
    /// </summary>
    /// <param name="index"> 文本块的索引 </param>
    /// <returns> 配置好的文本块 </returns>
    private TextBlock CreateTextBlock(int index)
    {
        var textBlock = new TextBlock
        {
            Text = Text.Substring(index / 2, 1),
            VerticalAlignment = VerticalContentAlignment
        };

        // 使用数据绑定来自动更新文本块的属性
        textBlock.SetBinding(TextBlock.FontSizeProperty, new Binding(nameof(FontSize)) { Source = this });
        textBlock.SetBinding(TextBlock.FontWeightProperty, new Binding(nameof(FontWeight)) { Source = this });
        textBlock.SetBinding(TextBlock.FontFamilyProperty, new Binding(nameof(FontFamily)) { Source = this });
        textBlock.SetBinding(TextBlock.FontStretchProperty, new Binding(nameof(FontStretch)) { Source = this });
        textBlock.SetBinding(TextBlock.FontStyleProperty, new Binding(nameof(FontStyle)) { Source = this });
        textBlock.SetBinding(TextBlock.ForegroundProperty, new Binding(nameof(Foreground)) { Source = this });

        return textBlock;
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        MainGrid = GetTemplateChild("Part_MainGrid") as Grid;
    }

    #region Depe

    /// <summary>
    ///     显示内容
    /// </summary>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text), typeof(string), typeof(TextShow), new PropertyMetadata(default(string), PropertyChanged));

    /// <summary>
    ///     显示内容
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    #endregion
}
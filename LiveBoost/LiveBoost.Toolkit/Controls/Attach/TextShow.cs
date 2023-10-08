// 创建时间：2023-03-14-9:33
// 修改时间：2023-03-15-9:51

namespace LiveBoost.Toolkit.Controls;

[TemplatePart(Name = "Part_MainGrid", Type = typeof(Grid))]
public class TextShow : Control
{
    #region Property

    private Grid? MainGrid;

    #endregion

    public TextShow()
    {
        SizeChanged += (_, _) => { ResetGrid(); };
    }

    private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextShow {MainGrid: not null} textShow) textShow.ResetGrid();
    }

    private void ResetGrid()
    {
        if (MainGrid is null) return;
        MainGrid.Children.Clear();
        MainGrid.ColumnDefinitions.Clear();
        if (string.IsNullOrEmpty(Text)) return;

        for (var i = 0; i < Text.Length * 2 - 1; i++)
            if (i % 2 == 1)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)});
            }
            else
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition {Width = GridLength.Auto});
                var textBlock = new TextBlock
                {
                    Text = Text.Substring(i / 2, 1), FontSize = FontSize, FontWeight = FontWeight,
                    FontFamily = FontFamily, FontStretch = FontStretch,FontStyle = FontStyle,
                    Foreground = Foreground, VerticalAlignment = VerticalContentAlignment
                };
                MainGrid.Children.Add(textBlock);
                Grid.SetColumn(textBlock, MainGrid.ColumnDefinitions.Count - 1);
            }
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
        get => (string) GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    #endregion
}

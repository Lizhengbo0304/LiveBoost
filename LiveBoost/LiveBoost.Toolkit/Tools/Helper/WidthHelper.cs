// 创建时间：2023-09-04-15:59
// 修改时间：2023-10-13-16:17

namespace LiveBoost.Toolkit.Tools;

public static class WidthHelper
{
    /// <summary>
    ///     获取文本的宽度
    /// </summary>
    /// <param name="text"> 文本内容 </param>
    /// <param name="weight"> 字体粗细 </param>
    /// <param name="fontFamily"> 字体族 </param>
    /// <param name="size"> 字体大小，默认为14 </param>
    /// <returns> 文本的宽度 </returns>
    public static double GetTextWidth(this string text, FontWeight weight, FontFamily fontFamily, double size = 14)
    {
        var textBlock = new TextBlock
        {
            Text = text,
            TextWrapping = TextWrapping.NoWrap,
            FontSize = size,
            HorizontalAlignment = HorizontalAlignment.Center,
            FontFamily = fontFamily,
            FontWeight = weight
        };

        // 测量文本块的大小
        textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

        // 对文本块进行布局
        textBlock.Arrange(new Rect(textBlock.DesiredSize));

        // 返回文本块的实际宽度
        return textBlock.ActualWidth;
    }

    /// <summary>
    ///     获取TabItem的宽度
    /// </summary>
    /// <param name="style"> TabItem的样式 </param>
    /// <param name="dataContext"> TabItem的数据上下文 </param>
    /// <returns> TabItem的宽度 </returns>
    public static double GetTabItemWidth(Style style, object dataContext)
    {
        var tabItem = new TabItem
        {
            Style = style,
            DataContext = dataContext
        };

        // 测量TabItem的大小
        tabItem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

        // 对TabItem进行布局
        tabItem.Arrange(new Rect(tabItem.DesiredSize));

        // 返回TabItem的实际宽度
        return tabItem.ActualWidth;
    }
}
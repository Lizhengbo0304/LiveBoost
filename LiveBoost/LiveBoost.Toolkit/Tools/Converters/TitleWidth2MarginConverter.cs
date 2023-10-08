﻿// 创建时间：2023-09-27-16:42
// 修改时间：2023-09-27-16:42

using System.Globalization;
using System.Windows.Data;
using HandyControl.Controls;

namespace LiveBoost.Toolkit.Tools;

public class TitleWidth2MarginConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ( value is not DependencyObject dependencyObject )
        {
            return new Thickness(2, 2, 0, 0);
        }
        var width = TitleElement.GetTitleWidth(dependencyObject).Value;
        return new Thickness(width + 2, 2, 0, 0);
    }

    /// <inheritdoc />
    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}
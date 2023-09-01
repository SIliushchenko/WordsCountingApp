﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Shell.Converters;

public sealed class NullToVisibilityConverter : IValueConverter
{
    public bool IsHidden { get; set; }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value == null ? IsHidden ? Visibility.Hidden : Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
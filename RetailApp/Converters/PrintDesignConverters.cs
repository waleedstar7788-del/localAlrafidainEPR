using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace RetailApp.Converters
{
    public class StringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string colorString && !string.IsNullOrWhiteSpace(colorString))
            {
                try
                {
                    if (!colorString.StartsWith("#")) colorString = "#" + colorString;
                    var brush = (Brush)new BrushConverter().ConvertFromString(colorString);
                    if (brush != null)
                    {
                        // Freeze the brush for better performance
                        brush.Freeze();
                        return brush;
                    }
                }
                catch
                {
                    // Ignore conversion errors and fall through to default
                }
            }
            return Brushes.Black; // Default fallback
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string marginString && !string.IsNullOrWhiteSpace(marginString))
            {
                try
                {
                    return new ThicknessConverter().ConvertFromString(marginString);
                }
                catch
                {
                    // Ignore errors
                }
            }
            return new Thickness(40); // Default fallback
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Thickness thickness)
            {
                return $"{thickness.Left},{thickness.Top},{thickness.Right},{thickness.Bottom}";
            }
            return "40,40,40,40";
        }
    }

    public class BoolToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isBold && isBold)
                return FontWeights.Bold;
            return FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FontWeight fw)
                return fw == FontWeights.Bold;
            return false;
        }
    }

    public class BoolToFontStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isItalic && isItalic)
                return FontStyles.Italic;
            return FontStyles.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FontStyle fs)
                return fs == FontStyles.Italic;
            return false;
        }
    }

    public class BoolToTextDecorationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isUnderlined && isUnderlined)
                return TextDecorations.Underline;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == TextDecorations.Underline;
        }
    }
}

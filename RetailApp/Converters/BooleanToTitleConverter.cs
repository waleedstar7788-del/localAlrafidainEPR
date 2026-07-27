using System;
using System.Globalization;
using System.Windows.Data;

namespace RetailApp.Converters
{
    public class BooleanToTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isNew)
            {
                return isNew ? "إضافة عميل جديد" : "تعديل بيانات العميل";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

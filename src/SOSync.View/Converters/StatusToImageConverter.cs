using System.Globalization;

namespace SOSync.View.Converters;

internal class StatusToImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string status)
            return ImageSource.FromFile($"{status}.png");
        else
            return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

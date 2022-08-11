using System.Globalization;

namespace SOSync.Mobile.Converters;

internal class StatusToImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string status)
        {
            var imgSrc = ImageSource.FromFile($"{status}.png");
            return imgSrc;
        }
        else
            return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

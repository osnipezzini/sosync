using System.Globalization;

namespace SOSync.Mobile.Converters;

internal class StatusToImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string status)
        {
            var imgPath = DeviceInfo.Platform == DevicePlatform.WinUI ? $"{status}.png" : $"{status}.png";
            return ImageSource.FromFile(imgPath);
        }
        else
            return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

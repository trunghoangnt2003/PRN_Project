using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PRN_Project
{
    public class Base64ToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty((string)value))
            {
                string base64String = (string)value;
                byte[] imageData = System.Convert.FromBase64String(base64String);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(imageData);
                bitmapImage.EndInit();
                return bitmapImage;
            }
            return null;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); 
        }
        public BitmapImage ConvertToPreviewImage(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string base64String)
            {
                byte[] imageData = System.Convert.FromBase64String(base64String);
                BitmapImage previewImage = new BitmapImage();
                previewImage.BeginInit();
                previewImage.StreamSource = new MemoryStream(imageData);
                previewImage.DecodePixelWidth = 100; // Set the desired preview image width
                previewImage.DecodePixelHeight = 100; // Set the desired preview image height
                previewImage.EndInit();
                return previewImage;
            }
            return null;
        }
    }
}

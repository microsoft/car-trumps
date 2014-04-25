/**
 * Copyright (c) 2012-2014 Microsoft Mobile.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CarTrumps
{
    public class FilePathToImageConverter : IValueConverter
    {
        private static IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapImage image = null;
            string path = value as string;
            
            if (String.IsNullOrEmpty(path))
            {
                return null;
            }

            if (path.Contains("isostore:"))
            {
                using(var sourceFile = isf.OpenFile(path.Substring(9),System.IO.FileMode.Open,System.IO.FileAccess.Read))
                {
                    image = new BitmapImage();
                    image.SetSource(sourceFile);
                    return image;
                }
            }
            else
            {
                image = new BitmapImage(new Uri(path));
                return image;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}

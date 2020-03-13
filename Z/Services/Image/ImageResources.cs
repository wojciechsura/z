using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Z.BusinessLogic.Services.Image;

namespace Z.Services.Image
{
    public class ImageResources : IImageResources
    {
        private string Prefix = "pack://application:,,,/Z;component/Resources/Images/";

        public ImageSource GetIconByName(string resourceName)
        {
            if (String.IsNullOrEmpty(resourceName))
                return null;

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(Prefix + resourceName);
            image.EndInit();

            return image;
        }
    }
}

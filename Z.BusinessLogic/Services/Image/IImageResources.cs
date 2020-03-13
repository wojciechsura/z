using System.Windows.Media;

namespace Z.BusinessLogic.Services.Image
{
    public interface IImageResources
    {
        ImageSource GetIconByName(string resourceName);
    }
}
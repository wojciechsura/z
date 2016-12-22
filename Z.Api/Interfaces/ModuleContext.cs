using System.IO;

namespace Z.Api.Interfaces
{
    public interface IModuleContext
    {
        Stream OpenFile(string filename);
    }
}

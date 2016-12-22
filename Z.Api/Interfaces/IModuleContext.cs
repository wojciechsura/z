using System.IO;

namespace Z.Api.Interfaces
{
    public interface IModuleContext
    {
        FileStream OpenConfigurationFile(string filename, FileMode fileMode, FileAccess fileAccess);
    }
}

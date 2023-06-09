﻿using System.IO;

namespace Z.Api.Interfaces
{
    public interface IModuleContext
    {
        /// <summary>
        /// Checks, if a file exists in module's private directory.
        /// </summary>
        /// <param name="filename">Name of file. Must be valid filename, without path.</param>
        /// <returns>True if file exists, otherwise false.</returns>
        bool ConfigurationFileExists(string filename);        

        /// <summary>
        /// Opens a filestream to file in module's private directory.
        /// </summary>
        /// <param name="filename">Name of file. Must be valid filename, without path.</param>
        /// <param name="fileMode">FileMode value, passed to created FileStream</param>
        /// <param name="fileAccess">FileAccess value, passed to created FileStream</param>
        /// <returns>FileStream to specified file.</returns>
        FileStream OpenConfigurationFile(string filename, FileMode fileMode, FileAccess fileAccess);
    }
}

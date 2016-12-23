using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Api.Interfaces;

namespace Z.Api
{
    public interface IZConfigurable
    {
        /// <summary>
        /// Returns object, which is used to display and manage module's configuration.
        /// </summary>
        IConfigurationProvider GetConfigurationProvider();
    }
}

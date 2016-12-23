using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.Api.Interfaces;

namespace Z.Api
{
    public interface IZInitializable
    {
        /// <summary>
        /// Called once, after registering module in application. May be used to load configuration.
        /// </summary>
        /// <param name="context">Gives access, to some application resources. May be stored.</param>
        void Initialize(IModuleContext context);

        /// <summary>
        /// Called once, just before application closing. May be used to free resources, store some data etc.
        /// </summary>
        void Deinitialize();
    }
}

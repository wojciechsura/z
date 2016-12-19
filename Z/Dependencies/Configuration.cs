using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.Dependencies
{
    public static class Configuration
    {
        public static void Configure()
        {           
            BusinessLogic.Dependencies.Configuration.Configure(Container.Instance);
        }

        public static void Dispose()
        {
            Container.Dispose();
        }
    }
}

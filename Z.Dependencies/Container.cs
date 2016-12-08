using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.Dependencies
{
    public static class Container
    {
        private static Lazy<UnityContainer> container = new Lazy<UnityContainer>(() => new UnityContainer());

        public static IUnityContainer Instance
        {
            get
            {
                return container.Value;
            }
        }

        public static void Dispose()
        {
            container.Value.Dispose();
            container = null;
        }
    }
}

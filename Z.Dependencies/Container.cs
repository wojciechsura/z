using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z.Dependencies
{
    public static class Container
    {
        private static IContainer container;

        public static void Configure(Action<ContainerBuilder> buildAction)
        {
            if (container != null)
                throw new InvalidOperationException("Container is already configured");

            var builder = new ContainerBuilder();
            builder.RegisterSource(new Autofac.Features.ResolveAnything.AnyConcreteTypeNotAlreadyRegisteredSource());
            buildAction(builder);
            container = builder.Build();
        }

        public static IContainer Instance => container;
    }
}

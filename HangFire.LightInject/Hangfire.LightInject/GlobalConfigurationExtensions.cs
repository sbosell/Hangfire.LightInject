using Hangfire.Annotations;
using Hangfire.LightInject;
using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hangfire
{
    public static class GlobalConfigurationExtensions
    {
        public static IGlobalConfiguration<LightInjectJobActivator> UseLightInjectActivator(
           [NotNull] this IGlobalConfiguration configuration,
           [NotNull] ServiceContainer container)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (container == null) throw new ArgumentNullException("container");

            return configuration.UseActivator(new LightInjectJobActivator(container));
        }
    }
}

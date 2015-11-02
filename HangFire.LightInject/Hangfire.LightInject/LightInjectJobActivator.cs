using Hangfire;
using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangfire.LightInject
{
    public class LightInjectJobActivator : JobActivator
    {
        private readonly ServiceContainer container;
        internal static readonly object LifetimeScopeTag = new object();
   
        public LightInjectJobActivator(ServiceContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            this.container = container;
        }

        public override object ActivateJob(Type jobType)
        {
            return container.GetInstance(jobType);
            //return container.GetInstance(jobType.GetInterfaces().FirstOrDefault());
        }

        public override JobActivatorScope BeginScope()
        {
            return new LightInjecterScope(container);
        }

    }

    class LightInjecterScope : JobActivatorScope
    {
        private readonly ServiceContainer _container;

        public LightInjecterScope(ServiceContainer container)
        {
            _container = container;
            _container.BeginScope();
        }

        public override object Resolve(Type type)
        {

             return _container.GetInstance(type);
        }

        public override void DisposeScope()
        {
            var scope = _container.ScopeManagerProvider.GetScopeManager().CurrentScope;
            if (scope != null)
            {
                _container.EndCurrentScope();
                scope.Dispose();
            }
        }
    }
}

using LightInject;
using System;
using System.Linq;

namespace Hangfire.LightInject
{
    public class LightInjectJobActivator : JobActivator
    {
        private readonly ServiceContainer _container;
        internal static readonly object LifetimeScopeTag = new object();

        public LightInjectJobActivator(ServiceContainer container, bool selfReferencing = false)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            this._container = container;

        }

        public override object ActivateJob(Type jobType)
        {
            // this will fail if you do self referencing job queues on a class with an interface:
            //  BackgroundJob.Enqueue(() => this.SendSms(message)); 
            var instance = _container.TryGetInstance(jobType);

            // since it fails we can try to get the first interface and request from container
            if (instance == null && jobType.GetInterfaces().Count() > 0)
                instance = _container.GetInstance(jobType.GetInterfaces().FirstOrDefault());

            return instance;

        }

        public override JobActivatorScope BeginScope()
        {
            return new LightInjecterScope(_container);
        }

    }

    class LightInjecterScope : JobActivatorScope
    {
        private readonly ServiceContainer _container;
        private readonly Scope _scope;

        public LightInjecterScope(ServiceContainer container)
        {
            _container = container;

            _scope = _container.BeginScope();
        }

        public override object Resolve(Type jobType)
        {

            var instance = _container.TryGetInstance(jobType);

            // since it fails we can try to get the first interface and request from container
            if (instance == null && jobType.GetInterfaces().Count() > 0)
                instance = _container.GetInstance(jobType.GetInterfaces().FirstOrDefault());

            return instance;

        }

        public override void DisposeScope()
        {
            _scope?.Dispose();
        }
    }
}

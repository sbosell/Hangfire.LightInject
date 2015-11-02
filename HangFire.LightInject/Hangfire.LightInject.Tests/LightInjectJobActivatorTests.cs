using LightInject;
using Hangfire.LightInject;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hangfire.LightInject.Tests
{
    public interface IFoo
    {

    }
    public class Foo : IFoo { }

    [TestClass]
    public class LightInjectJobActivatorTests
    {
        private ServiceContainer _container;

        [TestInitialize]
        public void SetUp()
        {
            _container = new ServiceContainer();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_ThrowsAnException_WhenLifetimeScopeIsNull()
        {
            // ReSharper disable once UnusedVariable
            var activator = new LightInjectJobActivator(null);
        }

        [TestMethod]
        public void Class_IsBasedOnJobActivator()
        {
            var activator = CreateActivator();
            Assert.IsInstanceOfType(activator, typeof(JobActivator));
        }

        [TestMethod]
        public void ActivateJob_ResolvesAnInstance_UsingLightInject()
        {
            _container.RegisterInstance<string>("called");
            var activator = CreateActivator();

            var result = activator.ActivateJob(typeof(string));

            Assert.AreEqual("called", result);
        }

        [TestMethod]
        public void ActivateJob_ResolvesAnInterface_UsingLightInject()
        {
            _container.Register<IFoo, Foo>();
            var activator = CreateActivator();

            var result = activator.ActivateJob(typeof(IFoo));

            Assert.AreEqual(typeof(Foo), result.GetType());
        }


        [TestMethod]
        public void InstanceRegisteredWith_SingleInstance_IsNotDisposedOnScopeDisposal()
        {
            var disposable = new Disposable();
            _container.RegisterInstance<Disposable>(disposable);
            var activator = CreateActivator();

            using (var scope = activator.BeginScope())
            {
                var instance = scope.Resolve(typeof(Disposable));
                Assert.IsFalse(disposable.Disposed);
            }

            Assert.IsFalse(disposable.Disposed);
        }

        [TestMethod]
        public void InstancePerBackgroundJob_RegistersSameServiceInstance_ForTheSameScopeInstance()
        {
            _container.Register<object>((f)=>new object(), new PerScopeLifetime());

            var activator = CreateActivator();

            using (var scope = activator.BeginScope())
            {
                var instance1 = scope.Resolve(typeof(object));
                var instance2 = scope.Resolve(typeof(object));

                Assert.AreSame(instance1, instance2);
            }
        }

        [TestMethod]
        public void InstancePerBackgroundJob_RegistersDifferentServiceInstances_ForDifferentScopeInstances()
        {
            _container.Register<object>((f) => new object(), new PerScopeLifetime());
            var activator = CreateActivator();

            object instance1;
            using (var scope1 = activator.BeginScope())
            {
                instance1 = scope1.Resolve(typeof(object));
            }

            object instance2;
            using (var scope2 = activator.BeginScope())
            {
                instance2 = scope2.Resolve(typeof(object));
            }

            Assert.AreNotSame(instance1, instance2);
        }

        [TestMethod]
        public void InstanceRegisteredWith_InstancePerBackgroundJob_IsDisposedOnScopeDisposal()
        {
            var disposable = new Disposable();
            _container.Register<Disposable>((f) => disposable, new PerScopeLifetime());
            var activator = CreateActivator();

            using (var scope = activator.BeginScope())
            {
                var instance = scope.Resolve(typeof(Disposable));
            }

            Assert.IsTrue(disposable.Disposed);
        }
        /*
                [TestMethod]
                public void UseAutofacActivator_CallsUseActivatorCorrectly()
                {
                    var configuration = new Mock<IBootstrapperConfiguration>();
                    var lifetimeScope = new Mock<ILifetimeScope>();

                    configuration.Object.UseAutofacActivator(lifetimeScope.Object);

                    configuration.Verify(x => x.UseActivator(It.IsAny<AutofacJobActivator>()));
                }


                 */
        private LightInjectJobActivator CreateActivator()
        {
            return new LightInjectJobActivator(_container);
        }
        class Disposable : IDisposable
        {
            public bool Disposed { get; set; }

            public void Dispose()
            {
                Disposed = true;
            }
        }
    }
}

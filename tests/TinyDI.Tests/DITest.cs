using System.Collections;
using System.Reflection;
using TinyDI.Tests.Stubs;
using TinyDI.Tests.Stubs.Layered;

namespace TinyDI.Tests
{
    class DITest
    {
        private static readonly DI instance = DI.Container;
        
        [TearDown]
        public void TearDown()
        {
            Clear();
        }

        [Test]
        public void AddSingleton_Success()
        {
            instance.AddSingleton<IRandomProvider, RandomProvider>();
            
            Assert.That(instance.RegisteredTypes, Has.One.EqualTo(typeof(IRandomProvider)));
        }
        
        [Test]
        public void AddSingleton_Fails_WhenNotConcreteType()
        {
            string message = Assert.Throws<ArgumentException>(() => instance.AddSingleton<IRandomProvider, IRandomProvider>()).Message;
            Assert.That(message, Is.EqualTo("TinyDI.Tests.Stubs.IRandomProvider must be a concrete type."));
        }

        [Test]
        public void Resolve_Generic_Success_WithZeroArgConstructor()
        {
            instance.AddSingleton<IRandomProvider, RandomProvider>();
            IRandomProvider randomProvider = instance.Resolve<IRandomProvider>();
            
            Assert.That(randomProvider, Is.Not.Null);
        }
        
        [Test]
        public void Resolve_Generic_Success_WithArgConstructor_Stub()
        {
            instance.AddSingleton<IRandomProvider, RandomProvider>()
                .AddSingleton<ILogger, Logger>()
                .AddSingleton<ITestService, TestService>();
            
            ITestService testService = instance.Resolve<ITestService>();
            
            Assert.That(testService, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(testService.RandomProvider, Is.Not.Null);
                Assert.That(testService.Logger, Is.Not.Null);
            });
        }

        [Test]
        public void Resolve_Generic_Success_WithArgConstructor_WithAttribute()
        {
            IPreferredService preferredService = instance.AddSingleton<ILogger, Logger>()
                .AddSingleton<IPreferredService, PreferredService>()
                .Resolve<IPreferredService>();
            
            Assert.That(preferredService, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(preferredService.Logger, Is.Not.Null);
                Assert.That(preferredService.RandomProvider, Is.Null);
            });
        }
        

        [Test]
        public void Resolve_Generic_Fails_WithCircularDependency()
        {
            instance.AddSingleton<ICircularService, CircularService>();
            string message = Assert.Throws<InvalidOperationException>(() => instance.Resolve<ICircularService>()).Message;
            Assert.That(message, Is.EqualTo("Circular dependency detected: ICircularService -> ICircularService"));
        }

        [Test]
        public void AddSingleton_Fails_WithMultipleInjectAttributes()
        {
            string message = Assert.Throws<ArgumentException>(() => instance.AddSingleton<IMultipleInjectAttributeService, MultipleInjectAttributeService>()).Message;
            Assert.That(message, Is.EqualTo("Type TinyDI.Tests.Stubs.MultipleInjectAttributeService has multiple constructors marked with [Inject]."));
        }

        [Test]
        public void AddSingleton_TypeRegistration_ThrowsWhenServiceAlreadyRegistered()
        {
            instance.AddSingleton<ILogger, Logger>();
            
            string message = Assert.Throws<InvalidOperationException>(() => instance.AddSingleton<ILogger, Logger>()).Message;
            Assert.That(message, Is.EqualTo("A type of TinyDI.Tests.Stubs.ILogger has already been registered."));
        }

        [Test]
        public void AddSingleton_InstanceRegistration_ThrowsWhenServiceAlreadyRegistered()
        {
            instance.AddSingleton<ILogger, Logger>();
            
            string message = Assert.Throws<InvalidOperationException>(() => instance.AddSingleton<ILogger>(new Logger())).Message;
            Assert.That(message, Is.EqualTo("A type of TinyDI.Tests.Stubs.ILogger has already been registered."));
        }

        [Test]
        public void AddSingleton_FailedDuplicateRegistration_PreservesOriginalSingleton()
        {
            instance.AddSingleton<ILogger, Logger>();
            
            ILogger logger1 = instance.Resolve<ILogger>();

            Assert.Throws<InvalidOperationException>(() => instance.AddSingleton<ILogger, Logger>());
            ILogger logger2 = instance.Resolve<ILogger>();
            
            Assert.That(logger1, Is.SameAs(logger2));
        }

        [Test]
        public void Resolve_Twice_ReturnsSameInstance()
        {
            instance.AddSingleton<ILogger, Logger>();
            
            ILogger logger = instance.Resolve<ILogger>();
            ILogger logger2 = instance.Resolve<ILogger>();
            
            Assert.That(logger, Is.SameAs(logger2));
        }

        [Test]
        public void InjectedDependency_IsSameSingletonAcrossServices()
        {
            instance.AddSingleton<ILogger, Logger>()
                .AddSingleton<IRandomProvider, RandomProvider>()
                .AddSingleton<ITestService, TestService>()
                .AddSingleton<IPreferredService, PreferredService>();
            
            ITestService testService = instance.Resolve<ITestService>();
            IPreferredService preferredService = instance.Resolve<IPreferredService>();
            
            Assert.That(testService.Logger, Is.Not.Null);
            Assert.That(testService.Logger, Is.SameAs(preferredService.Logger));
        }

        [Test]
        public void AddSingleton_Instance_ResolveReturnsExactInstance()
        {
            IPreferredService preferredService = new PreferredService(new Logger(), new RandomProvider());
            
            IPreferredService resolvedService = instance.AddSingleton(preferredService)
                    .Resolve<IPreferredService>();
            
            Assert.That(resolvedService, Is.Not.Null);
            Assert.That(resolvedService, Is.SameAs(preferredService));
        }

        [Test]
        public void AddSingleton_SelfRegistration_ResolvesConcreteType()
        {
            Logger logger = instance.AddSingleton<Logger>()
                .Resolve<Logger>();
            
            Assert.That(logger, Is.Not.Null);
            Assert.That(logger, Is.InstanceOf<Logger>());
        }

        [Test]
        public void Resolve_UnregisteredService_ThrowsExpectedException()
        {
            instance.AddSingleton<ILogger, Logger>()
                .AddSingleton<IRandomProvider, RandomProvider>();

            string message = Assert.Throws<ArgumentException>(() => instance.Resolve<ITestService>()).Message;
            Assert.That(message, Is.EqualTo("Could not find registered implementation for TinyDI.Tests.Stubs.ITestService."));
        }

        [Test]
        public void Resolve_WithUnregisteredTransitiveDependency_ThrowsExpectedException()
        {
            instance.AddSingleton<ILayer1Service, Layer1Service>()
                .AddSingleton<ILayer2Service, Layer2Service>()
                .AddSingleton<ILayer3Service, Layer3Service>();

            string message = Assert.Throws<ArgumentException>(() => instance.Resolve<ILayer1Service>()).Message;
            Assert.That(message, Is.EqualTo("Could not find registered implementation for TinyDI.Tests.Stubs.Layered.ILayer4Service."));
        }

        [Test]
        public void Resolve_WithoutInject_UsesMostParameterizedConstructor()
        {
            IAnotherTestService anotherTestService = instance.AddSingleton<IRandomProvider, RandomProvider>()
                .AddSingleton<ILogger, Logger>()
                .AddSingleton<IAnotherTestService, AnotherTestService>()
                .Resolve<IAnotherTestService>();
            
            Assert.That(anotherTestService, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(anotherTestService.RandomProvider, Is.Not.Null);
                Assert.That(anotherTestService.Logger, Is.Not.Null);
            });
        }

        [Test]
        public void AddSingleton_AbstractImplementation_Throws()
        {
            string message = Assert.Throws<ArgumentException>(() => instance.AddSingleton<ILogger, AbstractLogger>()).Message;
            Assert.That(message, Is.EqualTo("TinyDI.Tests.Stubs.AbstractLogger must be a concrete type."));
        }

        [Test]
        public void Resolve_IndirectCircularDependency_ReportsFullCycle()
        {
            instance.AddSingleton<ILayer1Service, Layer1Service>()
                .AddSingleton<ILayer2Service, Layer2Service>()
                .AddSingleton<ILayer3Service, Layer3Service>()
                .AddSingleton<ILayer4Service, LoopedLayer4Service>();
            
            string message = Assert.Throws<InvalidOperationException>(() => instance.Resolve<ILayer1Service>()).Message;
            Assert.That(message, Is.EqualTo("Circular dependency detected: ILayer1Service -> ILayer2Service -> ILayer3Service -> ILayer4Service -> ILayer1Service"));
        }

        [Test]
        public void AddSingleton_NullInstance_DefinesExpectedBehavior()
        {
            Assert.Throws<ArgumentNullException>(() => instance.AddSingleton<ILogger>(null!));
        }

        static void Clear()
        {
            FieldInfo[] fieldInfos = typeof(DI).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                if (fieldInfo.GetValue(DI.Container) is IDictionary dictionary)
                {
                    dictionary.Clear();
                }
            }
        }
    }
}

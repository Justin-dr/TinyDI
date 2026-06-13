namespace TinyDI.Tests.Stubs
{
    internal class TestService : ITestService
    {
        public ILogger Logger { get; }

        public IRandomProvider RandomProvider { get; }

        [Inject]
        internal TestService(IRandomProvider randomProvider, ILogger logger)
        {
            RandomProvider = randomProvider;
            Logger = logger;
        }
    }
    
    internal interface ITestService
    {
        public ILogger Logger { get; }

        public IRandomProvider RandomProvider { get; }
    }
}
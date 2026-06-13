namespace TinyDI.Tests.Stubs
{
    public class AnotherTestService: IAnotherTestService
    {
        public IRandomProvider RandomProvider { get; }
        public ILogger Logger { get; }
        
        public AnotherTestService(IRandomProvider randomProvider, ILogger logger)
        {
            RandomProvider = randomProvider;
            Logger = logger;
        }
        
        public AnotherTestService(ILogger logger)
        {
            RandomProvider = null!;
            Logger = logger;
        }
    }

    public interface IAnotherTestService
    {
        IRandomProvider RandomProvider { get; }
        ILogger Logger { get; }
    }
}
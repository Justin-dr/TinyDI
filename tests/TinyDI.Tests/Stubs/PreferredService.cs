namespace TinyDI.Tests.Stubs
{
    public class PreferredService : IPreferredService
    {
        public ILogger Logger { get; }
        public IRandomProvider RandomProvider { get; }
        
        public PreferredService(ILogger logger, IRandomProvider randomProvider)
        {
            Logger = logger;
            RandomProvider = randomProvider;
        }

        [Inject]
        public PreferredService(ILogger logger)
        {
            Logger = logger;
            RandomProvider = null!;
        }
    }

    public interface IPreferredService
    {
        public ILogger Logger { get; }
        public IRandomProvider RandomProvider { get; }
    }
}
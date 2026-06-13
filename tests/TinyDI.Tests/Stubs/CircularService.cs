namespace TinyDI.Tests.Stubs
{
    public class CircularService : ICircularService
    {
        public ICircularService Service { get; }

        public CircularService(ICircularService service)
        {
            Service = service;
        }
    }
    
    public interface ICircularService
    {
        public ICircularService Service { get; }
    }
}
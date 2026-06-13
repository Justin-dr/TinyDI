namespace TinyDI.Tests.Stubs.Layered
{
    public class Layer1Service : ILayer1Service
    {
        public Layer1Service(ILayer2Service layer2Service)
        {
            
        }
    }

    public interface ILayer1Service
    {
        
    }
}
namespace TinyDI.Tests.Stubs.Layered
{
    public class Layer2Service : ILayer2Service
    {
        public Layer2Service(ILayer3Service layer3Service)
        {
            
        }
    }

    public interface ILayer2Service
    {
        
    }
}
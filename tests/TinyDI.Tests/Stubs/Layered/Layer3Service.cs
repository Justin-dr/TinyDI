namespace TinyDI.Tests.Stubs.Layered
{
    public class Layer3Service : ILayer3Service
    {
        public Layer3Service(ILayer4Service layer4Service)
        {
            
        }
    }

    public interface ILayer3Service
    {
        
    }
}
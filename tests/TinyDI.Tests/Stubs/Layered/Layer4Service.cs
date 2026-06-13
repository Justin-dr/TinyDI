namespace TinyDI.Tests.Stubs.Layered
{
    public class Layer4Service : ILayer4Service
    {
        public Layer4Service(IRandomProvider randomProvider)
        {
            
        }
    }

    public class LoopedLayer4Service : ILayer4Service
    {
        public LoopedLayer4Service(ILayer1Service layer4Service)
        {
            
        }
    }

    public interface ILayer4Service
    {
        
    }
}
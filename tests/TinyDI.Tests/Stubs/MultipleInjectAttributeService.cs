namespace TinyDI.Tests.Stubs
{
    public class MultipleInjectAttributeService : IMultipleInjectAttributeService
    {
        [Inject]
        public MultipleInjectAttributeService()
        {
            
        }

        [Inject]
        public MultipleInjectAttributeService(IRandomProvider randomProvider)
        {
            
        }
    }
    
    public interface IMultipleInjectAttributeService
    {
        
    }
}
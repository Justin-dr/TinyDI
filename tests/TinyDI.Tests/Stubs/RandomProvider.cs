namespace TinyDI.Tests.Stubs
{
    public class RandomProvider : IRandomProvider
    {
        private readonly Random _random = new Random();

        public int Next()
        {
            return _random.Next();
        }
    }
    
    public interface IRandomProvider
    {
        
    }
}
namespace TinyDI.Tests.Stubs
{
    public class Logger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
    
    public interface ILogger
    {
        public void Log(string message);
    }
}
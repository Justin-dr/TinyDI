namespace TinyDI.Tests.Stubs
{
    public abstract class AbstractLogger : ILogger
    {
        public abstract void Log(string message);
    }
}
namespace Aduboker.Core.Model;

public interface ILogger
{
    void Log(string message);
    void Log(string message, Exception exception);
}
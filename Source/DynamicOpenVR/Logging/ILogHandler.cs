namespace DynamicOpenVR.Logging
{
    public interface ILogHandler
    {
        void Trace(object message);
        void Debug(object message);
        void Info(object message);
        void Notice(object message);
        void Warn(object message);
        void Error(object message);
        void Critical(object message);
    }
}

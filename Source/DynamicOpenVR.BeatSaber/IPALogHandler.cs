using DynamicOpenVR.Logging;

namespace DynamicOpenVR.BeatSaber
{
    internal class IPALogHandler : ILogHandler
    {
        public void Trace(object message)
        {
            Plugin.logger.Trace(message?.ToString());
        }

        public void Debug(object message)
        {
            Plugin.logger.Debug(message?.ToString());
        }

        public void Info(object message)
        {
            Plugin.logger.Info(message?.ToString());
        }

        public void Notice(object message)
        {
            Plugin.logger.Notice(message?.ToString());
        }

        public void Warn(object message)
        {
            Plugin.logger.Warn(message?.ToString());
        }

        public void Error(object message)
        {
            Plugin.logger.Error(message?.ToString());
        }

        public void Critical(object message)
        {
            Plugin.logger.Critical(message?.ToString());
        }
    }
}

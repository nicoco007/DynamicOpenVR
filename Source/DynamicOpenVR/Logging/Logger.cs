namespace DynamicOpenVR.Logging
{
    public static class Logger
    {
        public static ILogHandler handler = new UnityDebugLogHandler();

        internal static void Trace(object message) => handler.Trace(message);
        internal static void Debug(object message) => handler.Debug(message);
        internal static void Info(object message) => handler.Info(message);
        internal static void Notice(object message) => handler.Notice(message);
        internal static void Warn(object message) => handler.Warn(message);
        internal static void Error(object message) => handler.Error(message);
        internal static void Critical(object message) => handler.Critical(message);
    }
}

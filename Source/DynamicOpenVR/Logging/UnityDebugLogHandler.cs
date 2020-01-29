namespace DynamicOpenVR.Logging
{
    internal class UnityDebugLogHandler : ILogHandler
    {
        public void Trace(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        public void Debug(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        public void Info(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        public void Notice(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        public void Warn(object message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        public void Error(object message)
        {
            UnityEngine.Debug.LogError(message);
        }

        public void Critical(object message)
        {
            UnityEngine.Debug.LogError(message);
        }
    }
}

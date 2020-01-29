// DynamicOpenVR - Unity scripts to allow dynamic creation of OpenVR actions at runtime.
// Copyright © 2019-2020 Nicolas Gnyra

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.

// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.

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

// DynamicOpenVR.BeatSaber - An implementation of DynamicOpenVR as a Beat Saber plugin.
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

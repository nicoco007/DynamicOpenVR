// <copyright file="IPALogHandler.cs" company="Nicolas Gnyra">
// DynamicOpenVR.BeatSaber - An implementation of DynamicOpenVR as a Beat Saber plugin.
// Copyright © 2019-2021 Nicolas Gnyra
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>

using DynamicOpenVR.Logging;
using Logger = IPA.Logging.Logger;

namespace DynamicOpenVR.BeatSaber
{
    internal class IPALogHandler : ILogHandler
    {
        private readonly Logger _logger;

        public IPALogHandler(Logger logger)
        {
            _logger = logger;
        }

        public void Trace(object message)
        {
            _logger.Trace(message?.ToString());
        }

        public void Debug(object message)
        {
            _logger.Debug(message?.ToString());
        }

        public void Info(object message)
        {
            _logger.Info(message?.ToString());
        }

        public void Notice(object message)
        {
            _logger.Notice(message?.ToString());
        }

        public void Warn(object message)
        {
            _logger.Warn(message?.ToString());
        }

        public void Error(object message)
        {
            _logger.Error(message?.ToString());
        }

        public void Critical(object message)
        {
            _logger.Critical(message?.ToString());
        }
    }
}

// DynamicOpenVR - Unity scripts to allow dynamic creation of OpenVR actions at runtime.
// Copyright © 2019 Nicolas Gnyra

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

using UnityEngine;
using Valve.VR;

namespace DynamicOpenVR.IO
{
    public abstract class AnalogInput : Input
    {
        private int _lastFrame;
        private InputAnalogActionData_t _actionData;

        protected InputAnalogActionData_t actionData
        {
            get
            {
                if (_lastFrame != Time.frameCount)
                {
                    _actionData = OpenVrWrapper.GetAnalogActionData(handle);
                }

                _lastFrame = Time.frameCount;

                return _actionData;
            }
        }

        protected AnalogInput(string name) : base(name) { }

        /// <summary>
        /// Is set to True if this action is bound to an input source that is present in the system and is in an action set that is active.
        /// </summary>
        public override bool IsActive()
        {
            return actionData.bActive;
        }
    }
}

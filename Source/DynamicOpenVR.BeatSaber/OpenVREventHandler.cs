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

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Valve.VR;

namespace DynamicOpenVR.BeatSaber
{
    internal class OpenVREventHandler : MonoBehaviour
    {
        private readonly HashSet<EVREventType> _pauseEvents = new HashSet<EVREventType>(new [] { EVREventType.VREvent_InputFocusCaptured, EVREventType.VREvent_DashboardActivated, EVREventType.VREvent_OverlayShown });

        public event Action gamePaused;

        private VREvent_t _evt;
        private uint _size;

        private void Start()
        {
            _evt = default;
            _size = (uint)Marshal.SizeOf<VREvent_t>();
        }

        private void Update()
        {
            while (OpenVR.System.PollNextEvent(ref _evt, _size))
            {
                if (_pauseEvents.Contains((EVREventType) _evt.eventType))
                {
                    gamePaused?.Invoke();
                }
            }
        }
    }
}

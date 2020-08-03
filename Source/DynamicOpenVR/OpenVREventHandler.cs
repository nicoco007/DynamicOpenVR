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
using System.Runtime.InteropServices;
using UnityEngine;
using Valve.VR;
using Logger = DynamicOpenVR.Logging.Logger;

namespace DynamicOpenVR
{
    public class OpenVREventHandler : MonoBehaviour
    {
        private static OpenVREventHandler _instance;

        public static OpenVREventHandler instance
        {
            get
            {
                // check for null since we don't want to create another object if the current one is marked for destruction
                if (_instance == null)
                {
                    Logger.Info($"Creating instance of {nameof(OpenVREventHandler)}");

                    GameObject go = new GameObject(nameof(OpenVREventHandler));
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<OpenVREventHandler>();
                }

                return _instance;
            }
        }

        public event Action<VREvent_t> eventTriggered;

        private uint _size;

        private void Start()
        {
            _size = (uint)Marshal.SizeOf<VREvent_t>();
        }

        private void Update()
        {
            VREvent_t evt = default;

            while (OpenVR.System.PollNextEvent(ref evt, _size))
            {
                eventTriggered?.Invoke(evt);
            }
        }
    }
}

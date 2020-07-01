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
using DynamicOpenVR.Exceptions;
using UnityEngine.XR;
using Valve.VR;

namespace DynamicOpenVR
{
    public static class OpenVRUtilities
    {
        public static bool isInitialized { get; private set; }

        public static void Init()
        {
            if (!string.Equals(XRSettings.loadedDeviceName, "OpenVR", StringComparison.InvariantCultureIgnoreCase)) throw new OpenVRInitException($"OpenVR is not the selected VR SDK ({XRSettings.loadedDeviceName})");
            if (!OpenVRFacade.IsRuntimeInstalled()) throw new OpenVRInitException("OpenVR runtime is not installed");

            EVRInitError error = EVRInitError.None;
            CVRSystem system = OpenVR.Init(ref error);
            
            if (error != EVRInitError.None)
            {
                throw new OpenVRInitException(error);
            }

            if (system == null)
            {
                throw new OpenVRInitException("OpenVR.Init returned null");
            }

            isInitialized = true;
        }
    }
}

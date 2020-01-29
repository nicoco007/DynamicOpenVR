﻿// DynamicOpenVR - Unity scripts to allow dynamic creation of OpenVR actions at runtime.
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
using System.IO;
using UnityEngine.XR;

namespace DynamicOpenVR
{
    public static class OpenVRStatus
    {
        public static readonly string kActionManifestPath = Path.Combine(Environment.CurrentDirectory, "DynamicOpenVR", "action_manifest.json");

        public static bool isRunning => errorMessage == null;

        public static string errorMessage
        {
            get
            {
                if (NativeMethods.LoadLibrary("openvr_api") == IntPtr.Zero) return "OpenVR API is not loaded";
                if (string.Compare(XRSettings.loadedDeviceName, "OpenVR", StringComparison.InvariantCultureIgnoreCase) != 0) return "OpenVR is not the selected VR SDK";
                if (!OpenVRWrapper.isRuntimeInstalled) return "OpenVR is not running";

                return null;
            }
        }
    }
}

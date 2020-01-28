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

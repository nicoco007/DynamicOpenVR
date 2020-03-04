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
            if (NativeMethods.LoadLibrary("openvr_api") == IntPtr.Zero) throw new OpenVRInitException("OpenVR API is not loaded");
            if (string.Compare(XRSettings.loadedDeviceName, "OpenVR", StringComparison.InvariantCultureIgnoreCase) != 0) throw new OpenVRInitException($"OpenVR is not the selected VR SDK ({XRSettings.loadedDeviceName})");
            if (!OpenVRWrapper.isRuntimeInstalled) throw new OpenVRInitException("OpenVR runtime is not installed");

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

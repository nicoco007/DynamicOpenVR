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

        private void Update()
        {
            VREvent_t evt = default;
            if (OpenVR.System.PollNextEvent(ref evt, (uint)Marshal.SizeOf(typeof(VREvent_t))) && _pauseEvents.Contains((EVREventType) evt.eventType))
            {
                gamePaused?.Invoke();
            }
        }
    }
}

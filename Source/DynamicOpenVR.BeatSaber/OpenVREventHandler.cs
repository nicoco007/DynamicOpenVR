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
            if (OpenVR.System.PollNextEvent(ref _evt, _size) && _pauseEvents.Contains((EVREventType) _evt.eventType))
            {
                gamePaused?.Invoke();
            }
        }
    }
}

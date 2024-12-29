// <copyright file="OpenVRTracker.cs" company="Nicolas Gnyra">
// DynamicOpenVR.BeatSaber - An implementation of DynamicOpenVR as a Beat Saber plugin.
// Copyright © 2019-2024 Nicolas Gnyra
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

using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using Valve.VR;

namespace DynamicOpenVR.BeatSaber.Input.Devices
{
    [InputControlLayout(stateType = typeof(OpenVRTrackerState), updateBeforeRender = true, canRunInBackground = true)]
    public class OpenVRTracker : XRTracker, IInputUpdateCallbackReceiver
    {
        private static readonly uint kInputOriginInfoStructSize = (uint)Marshal.SizeOf(typeof(InputOriginInfo_t));

        private ulong _handle;

        public void OnUpdate()
        {
            InputOriginInfo_t originInfo = GetOriginInfo(_handle);

            if (originInfo.devicePath == default)
            {
                return;
            }

            TrackedDevicePose_t pose = OpenVRLoaderWithInputSystem.currentPoses[originInfo.trackedDeviceIndex]; // TODO: this is kind of lame; is there a way to avoid a static property?
            HmdMatrix34_t matrix = pose.mDeviceToAbsoluteTracking;

            bool isTracked = pose.bPoseIsValid && pose.eTrackingResult is ETrackingResult.Running_OK or ETrackingResult.Running_OutOfRange or ETrackingResult.Calibrating_OutOfRange;
            OpenVRTrackerState state = new()
            {
                isTracked = isTracked,
                trackingState = isTracked ? InputTrackingState.Position | InputTrackingState.Rotation | InputTrackingState.Velocity | InputTrackingState.AngularVelocity : InputTrackingState.None,
                devicePosition = matrix.GetPosition(),
                deviceRotation = matrix.GetRotation(),
            };
            InputSystem.QueueStateEvent(this, state);
        }

        public override void FinishSetup()
        {
            base.FinishSetup();

            var descriptor = XRDeviceDescriptor.FromJson(description.capabilities);

            if (descriptor == null)
            {
                return;
            }

            var characteristics = (InputDeviceTrackerCharacteristics)descriptor.characteristics;

            InternedString trackerUsage = default;
            string path = null;

            if (characteristics.HasFlag(InputDeviceTrackerCharacteristics.TrackerLeftFoot))
            {
                path = OpenVR.k_pchPathUserFootLeft;
                trackerUsage = XRTrackerUsages.LeftFoot;
            }
            else if (characteristics.HasFlag(InputDeviceTrackerCharacteristics.TrackerRightFoot))
            {
                path = OpenVR.k_pchPathUserFootRight;
                trackerUsage = XRTrackerUsages.RightFoot;
            }
            else if (characteristics.HasFlag(InputDeviceTrackerCharacteristics.TrackerLeftShoulder))
            {
                path = OpenVR.k_pchPathUserShoulderLeft;
                trackerUsage = XRTrackerUsages.LeftShoulder;
            }
            else if (characteristics.HasFlag(InputDeviceTrackerCharacteristics.TrackerRightShoulder))
            {
                path = OpenVR.k_pchPathUserShoulderRight;
                trackerUsage = XRTrackerUsages.RightShoulder;
            }
            else if (characteristics.HasFlag(InputDeviceTrackerCharacteristics.TrackerLeftElbow))
            {
                path = OpenVR.k_pchPathUserElbowLeft;
                trackerUsage = XRTrackerUsages.LeftElbow;
            }
            else if (characteristics.HasFlag(InputDeviceTrackerCharacteristics.TrackerRightElbow))
            {
                path = OpenVR.k_pchPathUserElbowRight;
                trackerUsage = XRTrackerUsages.RightElbow;
            }
            else if (characteristics.HasFlag(InputDeviceTrackerCharacteristics.TrackerLeftKnee))
            {
                path = OpenVR.k_pchPathUserKneeLeft;
                trackerUsage = XRTrackerUsages.LeftKnee;
            }
            else if (characteristics.HasFlag(InputDeviceTrackerCharacteristics.TrackerRightKnee))
            {
                path = OpenVR.k_pchPathUserKneeRight;
                trackerUsage = XRTrackerUsages.RightKnee;
            }
            else if (characteristics.HasFlag(InputDeviceTrackerCharacteristics.TrackerLeftWrist))
            {
                path = OpenVR.k_pchPathUserWristLeft;
                trackerUsage = XRTrackerUsages.LeftWrist;
            }
            else if (characteristics.HasFlag(InputDeviceTrackerCharacteristics.TrackerRightWrist))
            {
                path = OpenVR.k_pchPathUserWristRight;
                trackerUsage = XRTrackerUsages.RightWrist;
            }
            else if (characteristics.HasFlag(InputDeviceTrackerCharacteristics.TrackerLeftAnkle))
            {
                path = OpenVR.k_pchPathUserAnkleLeft;
                trackerUsage = XRTrackerUsages.LeftAnkle;
            }
            else if (characteristics.HasFlag(InputDeviceTrackerCharacteristics.TrackerRightAnkle))
            {
                path = OpenVR.k_pchPathUserAnkleRight;
                trackerUsage = XRTrackerUsages.RightAnkle;
            }
            else if (characteristics.HasFlag(InputDeviceTrackerCharacteristics.TrackerWaist))
            {
                path = OpenVR.k_pchPathUserWaist;
                trackerUsage = XRTrackerUsages.Waist;
            }
            else if (characteristics.HasFlag(InputDeviceTrackerCharacteristics.TrackerChest))
            {
                path = OpenVR.k_pchPathUserChest;
                trackerUsage = XRTrackerUsages.Chest;
            }
            else if (characteristics.HasFlag(InputDeviceTrackerCharacteristics.TrackerCamera))
            {
                path = OpenVR.k_pchPathUserCamera;
                trackerUsage = XRTrackerUsages.Camera;
            }
            else if (characteristics.HasFlag(InputDeviceTrackerCharacteristics.TrackerKeyboard))
            {
                path = OpenVR.k_pchPathUserKeyboard;
                trackerUsage = XRTrackerUsages.Keyboard;
            }

            InputSystem.SetDeviceUsage(this, trackerUsage);
            _handle = GetDeviceHandle(path);
        }

        private static ulong GetDeviceHandle(string devicePath)
        {
            ulong handle = 0;
            EVRInputError error = OpenVR.Input.GetInputSourceHandle(devicePath, ref handle);

            if (error is not EVRInputError.None)
            {
                Debug.LogError($"Failed to get input source handle for '{devicePath}': {error}");
                return 0;
            }

            return handle;
        }

        private static InputOriginInfo_t GetOriginInfo(ulong handle)
        {
            InputOriginInfo_t originInfo = default;
            EVRInputError error = OpenVR.Input.GetOriginTrackedDeviceInfo(handle, ref originInfo, kInputOriginInfoStructSize);

            if (error is not EVRInputError.None)
            {
                if (error is not EVRInputError.NoData and not EVRInputError.InvalidHandle)
                {
                    Debug.LogError($"Failed to get origin tracked device info for {handle}: {error}");
                }

                return default;
            }

            return originInfo;
        }
    }
}

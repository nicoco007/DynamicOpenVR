// <copyright file="OpenVRHMD.cs" company="Nicolas Gnyra">
// DynamicOpenVR.BeatSaber - An implementation of DynamicOpenVR as a Beat Saber plugin.
// Copyright © 2019-2023 Nicolas Gnyra
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

extern alias UnityXROpenVR;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityXROpenVR::Valve.VR;

namespace DynamicOpenVR.BeatSaber.Input.Devices
{
    [InputControlLayout(stateType = typeof(OpenVRHMDState))]
    internal class OpenVRHMD : XRHMD, IInputUpdateCallbackReceiver
    {
        [InputControl]
        public ButtonControl userPresence { get; set; }

        public void OnUpdate()
        {
            TrackedDevicePose_t pose = OpenVRLoaderWithInputSystem.currentPoses[OpenVR.k_unTrackedDeviceIndex_Hmd];
            Matrix4x4 transform = OpenVRMatrixToUnity(pose.mDeviceToAbsoluteTracking);
            bool isTracked = pose.bPoseIsValid && pose.eTrackingResult is ETrackingResult.Running_OK or ETrackingResult.Running_OutOfRange or ETrackingResult.Calibrating_OutOfRange;

            Matrix4x4 leftEyeTransform = GetEyeTransform(transform, EVREye.Eye_Left);
            Matrix4x4 rightEyeTransform = GetEyeTransform(transform, EVREye.Eye_Right);

            OpenVRHMDState state = new()
            {
                isTracked = isTracked,
                trackingState = (int)(isTracked ? InputTrackingState.All : InputTrackingState.None),
                devicePosition = transform.GetPosition(),
                deviceRotation = transform.rotation,
                leftEyePosition = leftEyeTransform.GetPosition(),
                leftEyeRotation = leftEyeTransform.rotation,
                rightEyePosition = rightEyeTransform.GetPosition(),
                rightEyeRotation = rightEyeTransform.rotation,
                centerEyePosition = (leftEyeTransform.GetPosition() + rightEyeTransform.GetPosition()) * 0.5f,
                centerEyeRotation = Quaternion.Lerp(leftEyeTransform.rotation, rightEyeTransform.rotation, 0.5f),
                userPresence = OpenVR.System.IsInputAvailable() && !OpenVR.System.ShouldApplicationPause() && (!Plugin.beatSaberActions.headsetOnHead.isActive || Plugin.beatSaberActions.headsetOnHead.state),
            };

            InputSystem.QueueStateEvent(this, state);
        }

        public override void FinishSetup()
        {
            base.FinishSetup();

            userPresence = GetChildControl<ButtonControl>("userPresence");
        }

        private static Matrix4x4 GetEyeTransform(Matrix4x4 transform, EVREye eye)
        {
            Matrix4x4 eyeTransform = OpenVRMatrixToUnity(OpenVR.System.GetEyeToHeadTransform(eye));
            return transform * eyeTransform;
        }

        // https://github.com/ValveSoftware/unity-xr-plugin/blob/35e0a1dce5c96d8f4a56312ae84acfa8e4c05dc7/Providers/Input/Input.cpp#L431
        private static Matrix4x4 OpenVRMatrixToUnity(HmdMatrix34_t matrix)
        {
            return new Matrix4x4(
                new Vector4(matrix.m0, matrix.m4, -matrix.m8, 0),
                new Vector4(matrix.m1, matrix.m5, -matrix.m9, 0),
                new Vector4(-matrix.m2, -matrix.m6, matrix.m10, 0),
                new Vector4(matrix.m3, matrix.m7, -matrix.m11, 1));
        }
    }
}

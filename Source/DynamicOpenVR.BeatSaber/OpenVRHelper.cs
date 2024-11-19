// <copyright file="OpenVRHelper.cs" company="Nicolas Gnyra">
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

using System;
using System.Threading;
using System.Threading.Tasks;
using DynamicOpenVR.IO;
using IPA.Utilities.Async;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR;
using UnityXROpenVR::Unity.XR.OpenVR;
using UnityXROpenVR::Valve.VR;

namespace DynamicOpenVR.BeatSaber
{
    internal class OpenVRHelper : UnityXRHelper, IVRPlatformHelper
    {
        public new void Start()
        {
            OpenVREvents.AddListener(EVREventType.VREvent_InputFocusCaptured, OnInputFocusCaptured);
            OpenVREvents.AddListener(EVREventType.VREvent_InputFocusReleased, OnInputFocusReleased);
            OpenVREvents.AddListener(EVREventType.VREvent_DashboardActivated, OnDashboardActivated);
            OpenVREvents.AddListener(EVREventType.VREvent_DashboardDeactivated, OnDashboardDeactivated);
            base.Start();
        }

        public new void OnDestroy()
        {
            OpenVREvents.RemoveListener(EVREventType.VREvent_InputFocusCaptured, OnInputFocusCaptured);
            OpenVREvents.RemoveListener(EVREventType.VREvent_InputFocusReleased, OnInputFocusReleased);
            OpenVREvents.RemoveListener(EVREventType.VREvent_DashboardActivated, OnDashboardActivated);
            OpenVREvents.RemoveListener(EVREventType.VREvent_DashboardDeactivated, OnDashboardDeactivated);
            base.OnDestroy();
        }

        public new Vector2 GetAnyJoystickMaxAxis()
        {
            return new Vector2(
                AbsMax(Plugin.beatSaberActions.leftThumbstick.vector.x, Plugin.beatSaberActions.rightThumbstick.vector.x),
                AbsMax(Plugin.beatSaberActions.leftThumbstick.vector.y, Plugin.beatSaberActions.rightThumbstick.vector.y));
        }

        public new bool GetMenuButton()
        {
            return Plugin.beatSaberActions.leftMenuButton.state || Plugin.beatSaberActions.rightMenuButton.state;
        }

        public new bool GetMenuButtonDown()
        {
            return Plugin.beatSaberActions.leftMenuButton.enabledChange || Plugin.beatSaberActions.rightMenuButton.enabledChange;
        }

        public new bool GetNodePose(XRNode nodeType, int idx, out Vector3 pos, out Quaternion rot)
        {
            if (nodeType == XRNode.Head)
            {
                PoseDataFlags flags = PoseDataSource.GetDataFromSource(TrackedPoseDriver.TrackedPose.Head, out Pose pose);
                pos = pose.position;
                rot = pose.rotation;
                return flags.HasFlag(PoseDataFlags.Position) && flags.HasFlag(PoseDataFlags.Rotation);
            }

            PoseInput poseInput = nodeType switch
            {
                XRNode.LeftHand => Plugin.beatSaberActions.leftHandPose,
                XRNode.RightHand => Plugin.beatSaberActions.rightHandPose,
                _ => null,
            };

            if (poseInput == null)
            {
                pos = Vector3.zero;
                rot = Quaternion.identity;
                return false;
            }

            pos = poseInput.position;
            rot = poseInput.rotation;
            return true;
        }

        public new Vector2 GetThumbstickValue(XRNode node)
        {
            Vector2Input input = node switch
            {
                XRNode.LeftHand => Plugin.beatSaberActions.leftThumbstick,
                XRNode.RightHand => Plugin.beatSaberActions.rightThumbstick,
                _ => throw new InvalidOperationException($"Unexpected node '{node}'"),
            };

            if (input == null)
            {
                return Vector2.zero;
            }

            return input.vector;
        }

        public new float GetTriggerValue(XRNode node)
        {
            VectorInput input = node switch
            {
                XRNode.LeftHand => Plugin.beatSaberActions.leftTrigger,
                XRNode.RightHand => Plugin.beatSaberActions.rightTrigger,
                _ => throw new InvalidOperationException($"Unexpected node '{node}'"),
            };

            if (input == null)
            {
                return 0;
            }

            return input.value;
        }

        private void OnInputFocusCaptured(VREvent_t evt)
        {
            if (evt.data.process.oldPid == 0)
            {
                SetUserPresenceAsync(false);
            }
        }

        private void OnInputFocusReleased(VREvent_t evt)
        {
            if (evt.data.process.pid == 0)
            {
                SetUserPresenceAsync(true);
            }
        }

        private void OnDashboardActivated(VREvent_t evt) => SetUserPresenceAsync(false);

        private void OnDashboardDeactivated(VREvent_t evt) => SetUserPresenceAsync(true);

        // TODO: this sometimes hard crashes the game without the UnityMainThreadTaskScheduler, unclear why
        private void SetUserPresenceAsync(bool value) => UnityMainThreadTaskScheduler.Factory.StartNew(() => userPresence = value).ContinueWith(task => Debug.LogError(task.Exception), CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, UnityMainThreadTaskScheduler.Default);

        private float AbsMax(float a, float b) => Mathf.Abs(a) > Mathf.Abs(b) ? a : b;
    }
}

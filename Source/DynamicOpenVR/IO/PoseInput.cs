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

using UnityEngine;
using Valve.VR;

namespace DynamicOpenVR.IO
{
    public class PoseInput : OVRInput
    {
        /// <summary>
        /// Is set to True if this action is bound to an input source that is present in the system and is in an action set that is active.
        /// </summary>
        public bool active => _actionData.bActive;

        /// <summary>
        /// Whether the device is currently connected or not.
        /// </summary>
        public bool deviceConnected => _actionData.pose.bDeviceIsConnected;

        public bool isPoseValid => _actionData.pose.bPoseIsValid;

        public Pose pose => _pose;

        public Vector3 position => _pose.position;

        public Quaternion rotation => _pose.rotation;

        public Vector3 velocity => ToVector3(_actionData.pose.vVelocity);

        public Vector3 angularVelocity => ToVector3(_actionData.pose.vAngularVelocity);

        /// <summary>
        /// Whether the device is currently tracking properly or not.
        /// </summary>
        /// <returns></returns>
        public bool isTracking => _actionData.pose.eTrackingResult == ETrackingResult.Running_OK;

        private InputPoseActionData_t _actionData;
        private Pose _pose;

        public PoseInput(string name) : base(name) { }

        public override bool isActive => _actionData.bActive;

        internal override void UpdateData()
        {
            _actionData = OpenVRWrapper.GetPoseActionDataForNextFrame(handle);
            HmdMatrix34_t rawMatrix = _actionData.pose.mDeviceToAbsoluteTracking;
            _pose = new Pose(rawMatrix.GetPosition(), rawMatrix.GetRotation());
        }

        private Vector3 ToVector3(HmdVector3_t vector)
        {
            return new Vector3(vector.v0, vector.v1, vector.v2);
        }
    }
}

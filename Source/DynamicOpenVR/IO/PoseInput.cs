// <copyright file="PoseInput.cs" company="Nicolas Gnyra">
// DynamicOpenVR - Unity scripts to allow dynamic creation of OpenVR actions at runtime.
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

using UnityEngine;
using Valve.VR;

namespace DynamicOpenVR.IO
{
    public class PoseInput : OVRInput
    {
        private InputPoseActionData_t _actionData;

        public PoseInput(string name)
            : base(name)
        {
        }

        /// <inheritdoc/>
        public override bool isActive => _actionData.bActive;

        /// <summary>
        /// Gets a value indicating whether the device is currently connected or not.
        /// </summary>
        public bool deviceConnected => _actionData.pose.bDeviceIsConnected;

        public bool isPoseValid => _actionData.pose.bPoseIsValid;

        public Pose pose { get; private set; }

        public Vector3 position { get; private set; }

        public Quaternion rotation { get; private set; }

        public Vector3 velocity { get; private set; }

        public Vector3 angularVelocity { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the device is currently tracking properly or not.
        /// </summary>
        public bool isTracking => _actionData.pose.bPoseIsValid && _actionData.pose.eTrackingResult is ETrackingResult.Running_OK or ETrackingResult.Running_OutOfRange or ETrackingResult.Calibrating_OutOfRange;

        /// <inheritdoc/>
        internal override void UpdateData(UpdateType updateType)
        {
            _actionData = OpenVRFacade.GetPoseActionData(handle, updateType);
            HmdMatrix34_t rawMatrix = _actionData.pose.mDeviceToAbsoluteTracking;
            position = rawMatrix.GetPosition();
            rotation = rawMatrix.GetRotation();
            pose = new Pose(position, rotation);
            velocity = ToVector3(_actionData.pose.vVelocity);
            angularVelocity = ToVector3(_actionData.pose.vAngularVelocity);
        }

        private Vector3 ToVector3(HmdVector3_t vector)
        {
            return new Vector3(vector.v0, vector.v1, -vector.v2);
        }
    }
}

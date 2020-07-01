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
            _actionData = OpenVRFacade.GetPoseActionData(handle);
            HmdMatrix34_t rawMatrix = _actionData.pose.mDeviceToAbsoluteTracking;
            _pose = new Pose(GetPosition(rawMatrix), GetRotation(rawMatrix));
        }

        private Vector3 GetPosition(HmdMatrix34_t rawMatrix)
        {
            return new Vector3(rawMatrix.m3, rawMatrix.m7, -rawMatrix.m11);
        }

        private Quaternion GetRotation(HmdMatrix34_t rawMatrix)
        {
            if (IsRotationValid(rawMatrix))
            {
                float w = Mathf.Sqrt(Mathf.Max(0, 1 + rawMatrix.m0 + rawMatrix.m5 + rawMatrix.m10)) / 2;
                float x = Mathf.Sqrt(Mathf.Max(0, 1 + rawMatrix.m0 - rawMatrix.m5 - rawMatrix.m10)) / 2;
                float y = Mathf.Sqrt(Mathf.Max(0, 1 - rawMatrix.m0 + rawMatrix.m5 - rawMatrix.m10)) / 2;
                float z = Mathf.Sqrt(Mathf.Max(0, 1 - rawMatrix.m0 - rawMatrix.m5 + rawMatrix.m10)) / 2;

                CopySign(ref x, rawMatrix.m6 - rawMatrix.m9);
                CopySign(ref y, rawMatrix.m8 - rawMatrix.m2);
                CopySign(ref z, rawMatrix.m4 - rawMatrix.m1);

                return new Quaternion(x, y, z, w);
            }

            return Quaternion.identity;
        }

        private static void CopySign(ref float sizeVal, float signVal)
        {
            if (signVal > 0 != sizeVal > 0)
                sizeVal = -sizeVal;
        }

        private Vector3 ToVector3(HmdVector3_t vector)
        {
            return new Vector3(vector.v0, vector.v1, vector.v2);
        }

        private bool IsRotationValid(HmdMatrix34_t rawMatrix)
        {
            return (rawMatrix.m2 != 0 || rawMatrix.m6 != 0 || rawMatrix.m10 != 0) && (rawMatrix.m1 != 0 || rawMatrix.m5 != 0 || rawMatrix.m9 != 0);
        }
    }
}

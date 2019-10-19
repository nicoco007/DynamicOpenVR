// DynamicOpenVR - Unity scripts to allow dynamic creation of OpenVR actions at runtime.
// Copyright © 2019 Nicolas Gnyra

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
    public class PoseInput : Input
    {
        public PoseInput(string name) : base(name) { }

        /// <summary>
        /// Is set to True if this action is bound to an input source that is present in the system and is in an action set that is active.
        /// </summary>
        public override bool IsActive()
        {
            return GetActionData().bActive;
        }

        /// <summary>
        /// Whether the device is currently connected or not.
        /// </summary>
        public bool IsDeviceConnected()
        {
            return GetActionData().pose.bDeviceIsConnected;
        }

        public bool IsPoseValid()
        {
            return GetActionData().pose.bPoseIsValid;
        }

        /// <summary>
        /// Whether the device is currently tracking properly or not.
        /// </summary>
        /// <returns></returns>
        public bool IsTracking()
        {
            return GetActionData().pose.eTrackingResult == ETrackingResult.Running_OK;
        }

        /// <summary>
        /// Retrieves the current pose.
        /// </summary>
        public Pose GetPose()
        {
            HmdMatrix34_t rawMatrix = GetActionData().pose.mDeviceToAbsoluteTracking;
            return new Pose(GetPosition(rawMatrix), GetRotation(rawMatrix));
        }

        private InputPoseActionData_t GetActionData()
        {
            return OpenVRWrapper.GetPoseActionDataForNextFrame(Handle);
        }

        private Vector3 GetPosition(HmdMatrix34_t rawMatrix)
        {
            return new Vector3(
                rawMatrix.m3,
                rawMatrix.m7,
                -rawMatrix.m11
            );
        }
        
        private Quaternion GetRotation(HmdMatrix34_t rawMatrix)
        {
            // this matrix transformation is based on the work from this fine person
            // https://github.com/wacki/Unity-VRInputModule/blob/master/Assets/SteamVR/Scripts/SteamVR_Utils.cs
            float[,] matrix = {
                {  rawMatrix.m0,  rawMatrix.m1, -rawMatrix.m2,   rawMatrix.m3 },
                {  rawMatrix.m4,  rawMatrix.m5, -rawMatrix.m6,   rawMatrix.m7 },
                { -rawMatrix.m8, -rawMatrix.m9,  rawMatrix.m10, -rawMatrix.m11 }
            };

            Quaternion rotation = new Quaternion(
                Mathf.Sqrt(Mathf.Max(0, 1 + matrix[0, 0] - matrix[1, 1] - matrix[2, 2])) / 2,
                Mathf.Sqrt(Mathf.Max(0, 1 - matrix[0, 0] + matrix[1, 1] - matrix[2, 2])) / 2,
                Mathf.Sqrt(Mathf.Max(0, 1 - matrix[0, 0] - matrix[1, 1] + matrix[2, 2])) / 2,
                Mathf.Sqrt(Mathf.Max(0, 1 + matrix[0, 0] + matrix[1, 1] + matrix[2, 2])) / 2
            );
            
            rotation.x = CopySign(rotation.x, matrix[2, 1] - matrix[1, 2]);
            rotation.y = CopySign(rotation.y, matrix[0, 2] - matrix[2, 0]);
            rotation.z = CopySign(rotation.z, matrix[1, 0] - matrix[0, 1]);

            return rotation;
        }

        private float CopySign(float magnitude, float sign)
        {
            return Mathf.Abs(magnitude) * Mathf.Sign(sign);
        }
    }
}

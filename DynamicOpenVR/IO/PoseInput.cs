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
        /// <summary>
        /// Is set to True if this action is bound to an input source that is present in the system and is in an action set that is active.
        /// </summary>
        public bool Active => actionData.bActive;

        /// <summary>
        /// Whether the device is currently connected or not.
        /// </summary>
        public bool DeviceConnected => ActionData.pose.bDeviceIsConnected;

        public bool IsPoseValid => ActionData.pose.bPoseIsValid;

        public Vector3 Velocity => ToVector3(ActionData.pose.vVelocity);

        public Vector3 AngularVelocity => ToVector3(ActionData.pose.vAngularVelocity);

        /// <summary>
        /// Whether the device is currently tracking properly or not.
        /// </summary>
        /// <returns></returns>
        public bool IsTracking => ActionData.pose.eTrackingResult == ETrackingResult.Running_OK;

        public Pose Pose
        {
            get
            {
                GetActionData();
                return pose;
            }
        }

        protected InputPoseActionData_t ActionData
        {
            get
            {
                GetActionData();
                return actionData;
            }
        }

        private int lastFrame;
        private InputPoseActionData_t actionData;
        private Pose pose;

        public PoseInput(string name) : base(name) { }

        public override bool IsActive()
        {
            return actionData.bActive;
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
            // based on SteamVR's Unity plugin
            // https://github.com/ValveSoftware/steamvr_unity_plugin/blob/master/Assets/SteamVR/Scripts/SteamVR_Utils.cs
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

        private Vector3 ToVector3(HmdVector3_t vector)
        {
            return new Vector3(vector.v0, vector.v1, vector.v2);
        }

        private void GetActionData()
        {
            if (lastFrame != Time.frameCount)
            {
                actionData = OpenVRWrapper.GetPoseActionDataForNextFrame(Handle);
                HmdMatrix34_t rawMatrix = actionData.pose.mDeviceToAbsoluteTracking;
                pose = new Pose(GetPosition(rawMatrix), GetRotation(rawMatrix));
            }

            lastFrame = Time.frameCount;
        }
    }
}

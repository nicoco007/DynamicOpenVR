using UnityEngine;

namespace DynamicOpenVR.IO
{
    public class PoseInput : OVRAction
    {
        public bool IsActive => PoseData.bActive;
        public bool IsDeviceConnected => PoseData.pose.bDeviceIsConnected;
        public bool IsPoseValid => PoseData.pose.bPoseIsValid;
        public bool IsTracking => PoseData.pose.eTrackingResult == ETrackingResult.Running_OK;

        private InputPoseActionData_t PoseData => OpenVRApi.GetPoseActionDataForNextFrame(Handle);

        public PoseInput(string name, OVRActionRequirement requirement = OVRActionRequirement.Suggested) : base(name, requirement, "pose", "in") { }

        public Pose GetPose()
        {
            HmdMatrix34_t rawMatrix = PoseData.pose.mDeviceToAbsoluteTracking;
            return new Pose(GetPosition(rawMatrix), GetRotation(rawMatrix));
        }

        public Vector3 GetPosition()
        {
            return GetPosition(PoseData.pose.mDeviceToAbsoluteTracking);
        }

        public Quaternion GetRotation()
        {
            return GetRotation(PoseData.pose.mDeviceToAbsoluteTracking);
        }

        // https://github.com/wacki/Unity-VRInputModule/blob/master/Assets/SteamVR/Scripts/SteamVR_Utils.cs
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

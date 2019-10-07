using UnityEngine;

namespace DynamicOpenVR.IO
{
    public class Vector3Input : OVRAction
	{
		public Vector3Input(string name, OVRActionRequirement requirement = OVRActionRequirement.Suggested) : base(name, requirement, "vector3", "in") { }

        public float GetAxisRawX()
        {
            return OpenVRApi.GetAnalogActionData(Handle).x;
        }

        public float GetAxisRawY()
        {
            return OpenVRApi.GetAnalogActionData(Handle).y;
        }

        public float GetAxisRawZ()
        {
            return OpenVRApi.GetAnalogActionData(Handle).z;
        }

        public Vector3 GetVector()
        {
            InputAnalogActionData_t actionData = OpenVRApi.GetAnalogActionData(Handle);
            return new Vector3(actionData.x, actionData.y, actionData.z);
        }
    }
}

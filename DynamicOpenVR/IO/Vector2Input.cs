using UnityEngine;

namespace DynamicOpenVR.IO
{
    public class Vector2Input : OVRAction
	{
		public Vector2Input(string name, OVRActionRequirement requirement = OVRActionRequirement.Suggested) : base(name, requirement, "vector2", "in") { }

        public float GetAxisRawX()
        {
            return OpenVRApi.GetAnalogActionData(Handle).x;
        }

        public float GetAxisRawY()
        {
            return OpenVRApi.GetAnalogActionData(Handle).y;
        }

        public Vector2 GetVector()
        {
            InputAnalogActionData_t actionData = OpenVRApi.GetAnalogActionData(Handle);
            return new Vector2(actionData.x, actionData.y);
        }
    }
}

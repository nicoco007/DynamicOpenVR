using UnityEngine;

namespace DynamicOpenVR.IO
{
    public class Vector3Input : AnalogInput
	{
		public Vector3Input(string name, OVRActionRequirement requirement = OVRActionRequirement.Suggested) : base(name, requirement, "vector3") { }

        /// <summary>
        /// The current state of this axis of the analog action.
        /// </summary>
        public Vector3 GetVector()
        {
            InputAnalogActionData_t actionData = GetActionData();
            return new Vector3(actionData.x, actionData.y, actionData.z);
        }

        /// <summary>
        /// The change in this axis for this action since the previous frame.
        /// </summary>
        public Vector3 GetVectorDelta()
        {
            InputAnalogActionData_t actionData = GetActionData();
            return new Vector3(actionData.deltaX, actionData.deltaY, actionData.deltaZ);
        }
    }
}

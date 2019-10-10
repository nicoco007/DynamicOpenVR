using UnityEngine;

namespace DynamicOpenVR.IO
{
    public class Vector2Input : AnalogInput
	{
		public Vector2Input(string name, OVRActionRequirement requirement = OVRActionRequirement.Suggested) : base(name, requirement, "vector2") { }

        /// <summary>
        /// The current state of this axis of the analog action.
        /// </summary>
        public Vector2 GetVector()
        {
            InputAnalogActionData_t actionData = GetActionData();
            return new Vector2(actionData.x, actionData.y);
        }

        /// <summary>
        /// The change in this axis for this action since the previous frame.
        /// </summary>
        public Vector2 GetVectorDelta()
        {
            InputAnalogActionData_t actionData = GetActionData();
            return new Vector2(actionData.deltaX, actionData.deltaY);
        }
    }
}

namespace DynamicOpenVR.IO
{
	public class VectorInput : AnalogInput
	{
		public VectorInput(string name, OVRActionRequirement requirement = OVRActionRequirement.Suggested) : base(name, requirement, "vector1") { }

        /// <summary>
        /// The current state of this axis of the analog action.
        /// </summary>
        public float GetValue()
        {
            return GetActionData().x;
        }

        /// <summary>
        /// The change in this axis for this action since the previous frame.
        /// </summary>
        public float GetValueDelta()
        {
            return GetActionData().deltaX;
        }
	}
}

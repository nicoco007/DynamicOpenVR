namespace DynamicOpenVR.IO
{
	public class BooleanInput : Input
	{
		public BooleanInput(string name, OVRActionRequirement requirement = OVRActionRequirement.Suggested) : base(name, requirement, "boolean") { }

        /// <summary>
        /// Is set to True if this action is bound to an input source that is present in the system and is in an action set that is active.
        /// </summary>
        public override bool IsActive()
        {
            return GetActionData().bActive;
        }

        /// <summary>
        /// The current state of this digital action. True means the user wants to perform this action.
        /// </summary>
		public bool GetState()
		{
            return GetActionData().bState;
		}

        /// <summary>
        /// If the state changed from disabled to enabled since it was last checked.
        /// </summary>
		public bool GetActiveChange()
		{
            InputDigitalActionData_t actionData = GetActionData();
			return actionData.bState && actionData.bChanged;
		}

        /// <summary>
        /// If the state changed from enabled to disabled since it was last checked.
        /// </summary>
        public bool GetInactiveChange()
        {
            InputDigitalActionData_t actionData = GetActionData();
			return !actionData.bState && actionData.bChanged;
		}

        private InputDigitalActionData_t GetActionData()
        {
            return OpenVRApi.GetDigitalActionData(Handle);
        }
	}
}

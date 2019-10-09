namespace DynamicOpenVR.IO
{
	public class BooleanInput : Input
	{
		private InputDigitalActionData_t Input => OpenVRApi.GetDigitalActionData(Handle);

		public BooleanInput(string name, OVRActionRequirement requirement = OVRActionRequirement.Suggested) : base(name, requirement, "boolean") { }

		public bool GetButton()
		{
			return Input.bState;
		}

		public bool GetButtonDown()
		{
			return Input.bState && Input.bChanged;
		}

		public bool GetButtonUp()
		{
			return !Input.bState && Input.bChanged;
		}
	}
}

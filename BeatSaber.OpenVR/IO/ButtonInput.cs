namespace BeatSaber.OpenVR.IO
{
	public class ButtonInput : OVRAction
	{
		private InputDigitalActionData_t Input => OpenVRApi.GetDigitalActionData(Handle);

		public ButtonInput(string name, OVRActionRequirement requirement = OVRActionRequirement.Suggested) : base(name, requirement, "boolean", "in") { }

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

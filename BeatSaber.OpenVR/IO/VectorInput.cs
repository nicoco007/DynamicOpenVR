namespace BeatSaber.OpenVR.IO
{
	class VectorInput : OVRAction
	{
		public VectorInput(string name, OVRActionRequirement requirement = OVRActionRequirement.Suggested) : base(name, requirement, "vector1", "in") { }

		public float GetAxisRaw()
		{
			return OpenVRApi.GetAnalogActionData(Handle).x;
		}
	}
}
